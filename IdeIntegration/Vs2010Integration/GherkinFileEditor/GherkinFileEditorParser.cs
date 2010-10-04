using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using gherkin;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.Gherkin;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;

namespace TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor
{
    internal class GherkinFileEditorParser
    {
        public static GherkinFileEditorParser GetParser(ITextBuffer buffer, IClassificationTypeRegistryService classificationRegistry, IVisualStudioTracer visualStudioTracer, SpecFlowProject specFlowProject)
        {
            return buffer.Properties.GetOrCreateSingletonProperty(() =>
                new GherkinFileEditorParser(buffer, classificationRegistry, visualStudioTracer, specFlowProject));
        }

        private class IdleHandler
        {
            private DateTime nextParseTime = DateTime.MinValue;

            public bool IsIdleTime()
            {
                return DateTime.Now >= nextParseTime;
            }

            public void SetIdleTime()
            {
                nextParseTime = DateTime.Now.AddMilliseconds(250);
            }

            public void WaitForIdle()
            {
                while (!IsIdleTime())
                {
                    Thread.Sleep(100);
                }
            }
        }

        private const string ParserTraceCategory = "Parser";

        private readonly ITextBuffer buffer;
        private readonly IVisualStudioTracer visualStudioTracer;
        private readonly SpecFlowProject specFlowProject;
        public GherkinFileEditorInfo GherkinFileEditorInfo { get; set; }

        readonly TaskFactory parsingTaskFactory = new TaskFactory();
        private readonly object pendingChangeInfoSynchRoot = new object();

        private readonly IdleHandler idleHandler = new IdleHandler();
        private Task parsingTask;
        private ChangeInfo pendingChangeInfo;
        private readonly GherkinFileEditorClassifications classifications;

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public GherkinFileEditorParser(ITextBuffer buffer, IClassificationTypeRegistryService registry, IVisualStudioTracer visualStudioTracer, SpecFlowProject specFlowProject)
        {
            this.buffer = buffer;
            this.visualStudioTracer = visualStudioTracer;
            this.specFlowProject = specFlowProject;
            this.buffer.Changed += BufferChanged;

            this.classifications = new GherkinFileEditorClassifications(registry);

            // initial parsing
            visualStudioTracer.Trace("Initial parsing scheduled", ParserTraceCategory);
            ChangeInfo changeInfo = new ChangeInfo(buffer);
            parsingTask = parsingTaskFactory.StartNew(() =>
                ParseAndTriggerChanges(GherkinFileEditorInfo, changeInfo));
        }

        private void BufferChanged(object sender, TextContentChangedEventArgs e)
        {
            bool isIdleTime = idleHandler.IsIdleTime();
            idleHandler.SetIdleTime();
            if (parsingTask == null || (parsingTask.IsCompleted && isIdleTime))
            {
                // no parsing in progress -> we start parsing
                visualStudioTracer.Trace("Trigger new parsing", ParserTraceCategory);
                ChangeInfo changeInfo = new ChangeInfo(e);
                parsingTask = parsingTaskFactory.StartNew(() => 
                    ParseAndTriggerChanges(GherkinFileEditorInfo, changeInfo));
                return;
            }

            // the pendingChangeInfo is a shared resource, we need to protect any read/write
            lock (pendingChangeInfoSynchRoot)
            {
                if (pendingChangeInfo == null)
                {
                    // parsing in progress, no pending request -> we queue up a pending request
                    visualStudioTracer.Trace("Queue new parsing", ParserTraceCategory);
                    pendingChangeInfo = new ChangeInfo(e);
                    parsingTask = parsingTask
                        .ContinueWith(prevTask =>
                                          {
                                              idleHandler.WaitForIdle();
                                              ParseAndTriggerChanges(GherkinFileEditorInfo,
                                                                         ConsumePendingChangeInfo());
                                          });
                }
                else
                {
                    // there is already a pending request -> we merge our new request into it
                    visualStudioTracer.Trace("Merge change to queued parse request", ParserTraceCategory);
                    pendingChangeInfo = pendingChangeInfo.Merge(e);
                }
            }
        }

        private ChangeInfo ConsumePendingChangeInfo()
        {
            // the pendingChangeInfo is a shared resource, we need to protect any read/write
            ChangeInfo changeInfo;
            lock (pendingChangeInfoSynchRoot)
            {
                changeInfo = pendingChangeInfo;
                pendingChangeInfo = null;
            }
            return changeInfo;
        }

        private void ParseAndTriggerChanges(GherkinFileEditorInfo gherkinFileEditorInfo, ChangeInfo changeInfo)
        {
            if (gherkinFileEditorInfo == null)
            {
                // initial parsing
                FullParse(changeInfo);
                return;
            }

            // incremental parsing
            var firstAffectedScenario = gherkinFileEditorInfo.ScenarioEditorInfos.LastOrDefault(
                s => s.StartLine <= changeInfo.ChangeFirstLine);

            if (firstAffectedScenario == null)
            {
                FullParse(changeInfo);
                return;
            }

            PartialParse(gherkinFileEditorInfo, changeInfo, firstAffectedScenario);
        }

        private void PartialParse(GherkinFileEditorInfo gherkinFileEditorInfo, ChangeInfo changeInfo, ScenarioEditorInfo firstAffectedScenario)
        {
            visualStudioTracer.Trace("Start incremental parsing", ParserTraceCategory);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int parseStartPosition = 
                changeInfo.TextSnapshot.GetLineFromLineNumber(firstAffectedScenario.StartLine).Start;

            string fileContent = changeInfo.TextSnapshot.GetText(parseStartPosition, changeInfo.TextSnapshot.Length - parseStartPosition);
            string fileHeader = changeInfo.TextSnapshot.GetText(0, parseStartPosition);
            I18n languageService = GetLanguageService(fileHeader);

            ScenarioEditorInfo firstUnchangedScenario;
            var partialResult = DoParsePartial(fileContent, languageService, 
                                               firstAffectedScenario.StartLine, 
                                               out firstUnchangedScenario, 
                                               changeInfo.TextSnapshot,
                                               gherkinFileEditorInfo,
                                               changeInfo.ChangeLastLine,
                                               changeInfo.LineCountDelta);

            if (partialResult.HeaderClassificationSpans.Any())
            {
                //TODO: merge to the prev scenario?
                partialResult.HeaderClassificationSpans.Clear();
            }
            partialResult.HeaderClassificationSpans.AddRange(
                gherkinFileEditorInfo.HeaderClassificationSpans
                    .Select(cs => cs.Shift(changeInfo.TextSnapshot, 0)));

            // inserting the non-affected scenarios
            partialResult.ScenarioEditorInfos.InsertRange(0,
                                                          gherkinFileEditorInfo.ScenarioEditorInfos.TakeUntilItemExclusive(firstAffectedScenario)
                                                              .Select(senario => senario.Shift(changeInfo.TextSnapshot, 0, 0)));

            if (firstUnchangedScenario != null)
            {
                // inserting the non-effected scenarios at the end

                partialResult.ScenarioEditorInfos.AddRange(
                    gherkinFileEditorInfo.ScenarioEditorInfos.SkipFromItemInclusive(firstUnchangedScenario)
                        .Select(
                            scenario =>
                            scenario.Shift(changeInfo.TextSnapshot, changeInfo.LineCountDelta, changeInfo.PositionDelta)));
            }

            TriggerChanges(partialResult, changeInfo, firstAffectedScenario, firstUnchangedScenario);

            stopwatch.Stop();
            visualStudioTracer.Trace("Finished incremental parsing in " + stopwatch.ElapsedMilliseconds + " ms", ParserTraceCategory);
        }

        private void FullParse(ChangeInfo changeInfo)
        {
            visualStudioTracer.Trace("Start full parsing", ParserTraceCategory);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string fileContent = changeInfo.TextSnapshot.GetText();
            I18n languageService = GetLanguageService(fileContent);

            var result = DoParse(fileContent, languageService, changeInfo.TextSnapshot);

            TriggerChanges(result, changeInfo);

            stopwatch.Stop();
            visualStudioTracer.Trace("Finished full parsing in " + stopwatch.ElapsedMilliseconds + " ms", ParserTraceCategory);
        }

        private void TriggerChanges(GherkinFileEditorInfo gherkinFileEditorInfo, ChangeInfo changeInfo, ScenarioEditorInfo firstAffectedScenario = null, ScenarioEditorInfo firstUnchangedScenario = null)
        {
            this.GherkinFileEditorInfo = gherkinFileEditorInfo;

            var textSnapshot = changeInfo.TextSnapshot;
            int startPosition = 0;
            if (firstAffectedScenario != null)
                startPosition = textSnapshot.GetLineFromLineNumber(firstAffectedScenario.StartLine).Start;
            int endPosition = textSnapshot.Length;
            if (firstUnchangedScenario != null)
                endPosition = textSnapshot.GetLineFromLineNumber(firstUnchangedScenario.StartLine).Start;
            var snapshotSpan = new SnapshotSpan(textSnapshot, startPosition, endPosition - startPosition);

            if (ClassificationChanged != null)
            {
                ClassificationChanged(this, new ClassificationChangedEventArgs(snapshotSpan));
            }
            if (TagsChanged != null)
            {
                TagsChanged(this, new SnapshotSpanEventArgs(snapshotSpan));
            }
        }

        private GherkinFileEditorInfo DoParsePartial(string fileContent, I18n languageService, int lineOffset, out ScenarioEditorInfo firstUnchangedScenario, ITextSnapshot textSnapshot, GherkinFileEditorInfo previousGherkinFileEditorInfo, int changeLastLine, int changeLineDelta)
        {
            GherkinScanner scanner = new GherkinScanner(languageService, fileContent, lineOffset);

            var gherkinListener = new GherkinFileEditorParserListener(textSnapshot, classifications, previousGherkinFileEditorInfo, changeLastLine, changeLineDelta);
            firstUnchangedScenario = null;
            try
            {
                scanner.Scan(gherkinListener);
            }
            catch (PartialListeningDoneException partialListeningDoneException)
            {
                firstUnchangedScenario = partialListeningDoneException.FirstUnchangedScenario;
            }

            return gherkinListener.GetResult();
        }

        private GherkinFileEditorInfo DoParse(string fileContent, I18n languageService, ITextSnapshot textSnapshot)
        {
            ScenarioEditorInfo firstUnchangedScenario;
            return DoParsePartial(fileContent, languageService, 0, out firstUnchangedScenario, textSnapshot, null, 0, 0);
        }

        private I18n GetLanguageService(string fileContent)
        {
            CultureInfo defaultLanguage = specFlowProject == null ? 
                CultureInfo.GetCultureInfo("en-US") : 
                specFlowProject.GeneratorConfiguration.FeatureLanguage;
            var languageServices = new GherkinLanguageServices(defaultLanguage);

            return languageServices.GetLanguageService(fileContent);
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            if (GherkinFileEditorInfo == null)
                return new ClassificationSpan[0];

            var result = new List<ClassificationSpan>();
            result.AddRange(GherkinFileEditorInfo.HeaderClassificationSpans);
            foreach (var scenarioEditorInfo in GherkinFileEditorInfo.ScenarioEditorInfos)
            {
                result.AddRange(scenarioEditorInfo.ClassificationSpans);
            }
            return result;
        }

        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (GherkinFileEditorInfo == null)
                return new ITagSpan<IOutliningRegionTag>[0];

            var result = new List<ITagSpan<IOutliningRegionTag>>();
            result.AddRange(GherkinFileEditorInfo.HeaderOutliningRegions);
            foreach (var scenarioEditorInfo in GherkinFileEditorInfo.ScenarioEditorInfos)
            {
                result.AddRange(scenarioEditorInfo.OutliningRegions);
            }
            return result;
        }
    }
}