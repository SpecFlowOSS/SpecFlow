using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        private const string ParserTraceCategory = "EditorParser";

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

        private int partialParseCount = 0;
        private const int PartialParseCountLimit = 30;

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

            if (partialParseCount >= PartialParseCountLimit)
            {
                visualStudioTracer.Trace("Forced full parse after " + partialParseCount + " incremental parse", ParserTraceCategory);
                FullParse(changeInfo);
                return;
            }

            // incremental parsing
            var firstAffectedScenario = gherkinFileEditorInfo.ScenarioEditorInfos.LastOrDefault(
                s => s.StartLine <= changeInfo.ChangeFirstLine);

            if (firstAffectedScenario == null)
            {
                // We would not need to do a full parse when the header chenges, but it would
                // be too complicated to bring this case through the logic now.
                // So the side-effect is that we do a full parse when the header changes instead of an incremental.
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

            partialParseCount++;

            int parseStartPosition = 
                changeInfo.TextSnapshot.GetLineFromLineNumber(firstAffectedScenario.StartLine).Start;

            string fileContent = changeInfo.TextSnapshot.GetText(parseStartPosition, changeInfo.TextSnapshot.Length - parseStartPosition);
            string fileHeader = changeInfo.TextSnapshot.GetText(0, parseStartPosition);
            var gherkinDialect = GetGherkinDialect(fileHeader);

            ScenarioEditorInfo firstUnchangedScenario;
            var partialResult = DoParsePartial(fileContent, gherkinDialect, 
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

            ScenarioEditorInfo firstUnchangedScenarioShifted = null;
            if (firstUnchangedScenario != null)
            {
                // inserting the non-effected scenarios at the end

                int firstNewScenarioIndex = partialResult.ScenarioEditorInfos.Count;
                partialResult.ScenarioEditorInfos.AddRange(
                    gherkinFileEditorInfo.ScenarioEditorInfos.SkipFromItemInclusive(firstUnchangedScenario)
                        .Select(
                            scenario =>
                            scenario.Shift(changeInfo.TextSnapshot, changeInfo.LineCountDelta, changeInfo.PositionDelta)));

                firstUnchangedScenarioShifted = partialResult.ScenarioEditorInfos.Count > firstNewScenarioIndex
                                             ? partialResult.ScenarioEditorInfos[firstNewScenarioIndex]
                                             : null;
            }

            TriggerChanges(partialResult, changeInfo, firstAffectedScenario, firstUnchangedScenarioShifted);

            stopwatch.Stop();
            TraceFinishParse(stopwatch, "incremental");
        }

        private GherkinFileEditorInfo DoFullParse(ITextSnapshot textSnapshot)
        {
            string fileContent = textSnapshot.GetText();
            var gherkinDialect = GetGherkinDialect(fileContent);

            return DoParse(fileContent, gherkinDialect, textSnapshot);
        }

        public GherkinFileEditorInfo EnsureParsingResult(ITextSnapshot textSnapshot)
        {
            if (this.GherkinFileEditorInfo != null)
                return this.GherkinFileEditorInfo;
            return DoFullParse(textSnapshot);
        }

        private void FullParse(ChangeInfo changeInfo)
        {
            visualStudioTracer.Trace("Start full parsing", ParserTraceCategory);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            partialParseCount = 0;

            var result = DoFullParse(changeInfo.TextSnapshot);

            TriggerChanges(result, changeInfo);

            stopwatch.Stop();
            TraceFinishParse(stopwatch, "full");
        }

        private void TraceFinishParse(Stopwatch stopwatch, string parseKind)
        {
            visualStudioTracer.Trace(
                string.Format("Finished {0} parsing in {1} ms, {2} errors", parseKind, stopwatch.ElapsedMilliseconds, GherkinFileEditorInfo.TotalErrorCount), ParserTraceCategory);
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

            // safety criteria to avoid argument execption in case of a wrong parser result
            if (startPosition >= endPosition)
                return;

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

        private GherkinFileEditorInfo DoParsePartial(string fileContent, GherkinDialect gherkinDialect, int lineOffset, out ScenarioEditorInfo firstUnchangedScenario, ITextSnapshot textSnapshot, GherkinFileEditorInfo previousGherkinFileEditorInfo, int changeLastLine, int changeLineDelta)
        {
            GherkinScanner scanner = new GherkinScanner(gherkinDialect, fileContent, lineOffset);

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

            var result = gherkinListener.GetResult();
            result.GherkinDialect = gherkinDialect;
            return result;
        }

        private GherkinFileEditorInfo DoParse(string fileContent, GherkinDialect gherkinDialect, ITextSnapshot textSnapshot)
        {
            ScenarioEditorInfo firstUnchangedScenario;
            return DoParsePartial(fileContent, gherkinDialect, 0, out firstUnchangedScenario, textSnapshot, null, 0, 0);
        }

        private GherkinDialect GetGherkinDialect(string fileContent)
        {
            CultureInfo defaultLanguage = specFlowProject == null ? 
                CultureInfo.GetCultureInfo("en-US") : 
                specFlowProject.GeneratorConfiguration.FeatureLanguage;
            var languageServices = new GherkinDialectServices(defaultLanguage);

            return languageServices.GetGherkinDialect(fileContent);
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