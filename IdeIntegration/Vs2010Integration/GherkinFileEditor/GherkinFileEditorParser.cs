using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using gherkin;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

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

        private readonly ITextBuffer buffer;
        public GherkinFileEditorInfo GherkinFileEditorInfo { get; set; }

        readonly TaskFactory parsingTaskFactory = new TaskFactory();
        private readonly object pendingChangeInfoSynchRoot = new object();

        private readonly IdleHandler idleHandler = new IdleHandler();
        private Task parsingTask;
        private ChangeInfo pendingChangeInfo;
        private readonly GherkinFileEditorClassifications classifications;

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        public GherkinFileEditorParser(ITextBuffer buffer, IClassificationTypeRegistryService registry)
        {
            this.buffer = buffer;
            this.buffer.Changed += BufferChanged;

            this.classifications = new GherkinFileEditorClassifications(registry);

            // initial parsing
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
            int parseStartPosition = 
                changeInfo.TextSnapshot.GetLineFromLineNumber(firstAffectedScenario.StartLine).Start;

            string fileContent = changeInfo.TextSnapshot.GetText(parseStartPosition, changeInfo.TextSnapshot.Length - parseStartPosition);
            I18n languageService = GetLanguageService();

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
        }

        private void FullParse(ChangeInfo changeInfo)
        {
            string fileContent = changeInfo.TextSnapshot.GetText();
            I18n languageService = GetLanguageService();

            var result = DoParse(fileContent, languageService, changeInfo.TextSnapshot);

            TriggerChanges(result, changeInfo);
        }

        private void TriggerChanges(GherkinFileEditorInfo gherkinFileEditorInfo, ChangeInfo changeInfo, ScenarioEditorInfo firstAffectedScenario = null, ScenarioEditorInfo firstUnchangedScenario = null)
        {
            this.GherkinFileEditorInfo = gherkinFileEditorInfo;

            if (ClassificationChanged != null)
            {
                var textSnapshot = changeInfo.TextSnapshot;
                int startPosition = 0;
                if (firstAffectedScenario != null)
                    startPosition = textSnapshot.GetLineFromLineNumber(firstAffectedScenario.StartLine).Start;
                int endPosition = textSnapshot.Length;
                if (firstUnchangedScenario != null)
                    endPosition = textSnapshot.GetLineFromLineNumber(firstUnchangedScenario.StartLine).Start;

                ClassificationChanged(this,
                                      new ClassificationChangedEventArgs(
                                          new SnapshotSpan(
                                              textSnapshot,
                                              startPosition,
                                              endPosition - startPosition)));
            }
        }

        private GherkinFileEditorInfo DoParsePartial(string fileContent, I18n languageService, int lineOffset, out ScenarioEditorInfo firstUnchangedScenario, ITextSnapshot textSnapshot, GherkinFileEditorInfo previousGherkinFileEditorInfo, int changeLastLine, int changeLineDelta)
        {
            var gherkinListener = new GherkinFileEditorParserListener(textSnapshot, classifications, previousGherkinFileEditorInfo, lineOffset, changeLastLine, changeLineDelta);
            return DoScan(fileContent, textSnapshot, lineOffset, languageService, gherkinListener, 0, out firstUnchangedScenario);
        }

        private GherkinFileEditorInfo DoScan(string fileContent, ITextSnapshot textSnapshot, int lineOffset, I18n languageService, GherkinFileEditorParserListener gherkinListener, int errorRertyCount, out ScenarioEditorInfo firstUnchangedScenario)
        {
            const int MAX_ERROR_RETRY = 5;
            const int NO_ERROR_RETRY_FOR_LINES = 5;

            firstUnchangedScenario = null;
            try
            {
                Lexer lexer = languageService.lexer(gherkinListener);
                lexer.scan(fileContent, null, 0);
            }
            catch (PartialListeningDoneException partialListeningDoneException)
            {
                firstUnchangedScenario = partialListeningDoneException.FirstUnchangedScenario;
            }
            catch(LexingError lexingError)
            {
                int? errorLine = GetErrorLine(lexingError, lineOffset);
                if (errorLine != null && 
                    errorLine.Value < textSnapshot.LineCount - NO_ERROR_RETRY_FOR_LINES &&
                    errorRertyCount < MAX_ERROR_RETRY)
                {
                    //add error classification & continue

                    var restartLineNumber = errorLine.Value + 1;
                    int restartPosition = textSnapshot.GetLineFromLineNumber(restartLineNumber).Start;
                    string restartFileContent = textSnapshot.GetText(restartPosition, textSnapshot.Length - restartPosition);

                    gherkinListener.LineOffset = restartLineNumber;
                    return DoScan(restartFileContent, textSnapshot, 
                                  restartLineNumber, languageService, gherkinListener, 
                                  errorRertyCount + 1,
                                  out firstUnchangedScenario);
                }
            }
// ReSharper disable EmptyGeneralCatchClause
            catch
// ReSharper restore EmptyGeneralCatchClause
            {
                // unknown error
            }

            return gherkinListener.GetResult();
        }

        private int? GetErrorLine(LexingError lexingError, int lineOffset)
        {
            Regex lineNoRe = new Regex(@"^Lexing error on line (?<lineno>\d+):");

            var match = lineNoRe.Match(lexingError.Message);
            if (!match.Success)
                return null;

            int parserdLine = int.Parse(match.Groups["lineno"].Value);
            return parserdLine - 1 + lineOffset;
        }

        private GherkinFileEditorInfo DoParse(string fileContent, I18n languageService, ITextSnapshot textSnapshot)
        {
            ScenarioEditorInfo firstUnchangedScenario;
            return DoParsePartial(fileContent, languageService, 0, out firstUnchangedScenario, textSnapshot, null, 0, 0);
        }

        private I18n GetLanguageService()
        {
            return new I18n("en");
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
    }
}