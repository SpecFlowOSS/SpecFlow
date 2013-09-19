using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.Text;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.Gherkin;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public class GherkinTextBufferParser
    {
        private const string ParserTraceCategory = "EditorParser";
        private const int PartialParseCountLimit = 30;

        private int partialParseCount = 0;
        private readonly IProjectScope projectScope;
        private readonly IVisualStudioTracer visualStudioTracer;

        public GherkinTextBufferParser(IProjectScope projectScope, IVisualStudioTracer visualStudioTracer)
        {
            this.projectScope = projectScope;
            this.visualStudioTracer = visualStudioTracer;

        }

        private GherkinDialect GetGherkinDialect(ITextSnapshot textSnapshot)
        {
            try
            {
                return projectScope.GherkinDialectServices.GetGherkinDialect(
                        lineNo => textSnapshot.GetLineFromLineNumber(lineNo).GetText());
            }
            catch(Exception)
            {
                return null;
            }
        }

        public GherkinFileScopeChange Parse(GherkinTextBufferChange change, IGherkinFileScope previousScope = null)
        {
            var gherkinDialect = GetGherkinDialect(change.ResultTextSnapshot);
            if (gherkinDialect == null)
                return GetInvalidDialectScopeChange(change);

            bool fullParse = false;
            if (previousScope == null)
                fullParse = true;
            else if (!Equals(previousScope.GherkinDialect, gherkinDialect))
                fullParse = true;
            else if (partialParseCount >= PartialParseCountLimit)
                fullParse = true;
            else if (GetFirstAffectedScenario(change, previousScope) == null)
                fullParse = true;

            if (fullParse)
                return FullParse(change.ResultTextSnapshot, gherkinDialect);

            return PartialParse(change, previousScope);
        }

        private GherkinFileScopeChange GetInvalidDialectScopeChange(GherkinTextBufferChange change)
        {
            var fileScope = new GherkinFileScope(null, change.ResultTextSnapshot)
                                {
                                    InvalidFileEndingBlock = new InvalidFileBlock(0, 
                                        change.ResultTextSnapshot.LineCount - 1,
                                        new ErrorInfo("Invalid Gherkin dialect!", 0, 0, null))
                                };

            return GherkinFileScopeChange.CreateEntireScopeChange(fileScope);
        }

        private GherkinFileScopeChange FullParse(ITextSnapshot textSnapshot, GherkinDialect gherkinDialect)
        {
            visualStudioTracer.Trace("Start full parsing", ParserTraceCategory);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            partialParseCount = 0;

            var gherkinListener = new GherkinTextBufferParserListener(gherkinDialect, textSnapshot, projectScope);

            var scanner = new GherkinScanner(gherkinDialect, textSnapshot.GetText(), 0);
            scanner.Scan(gherkinListener);

            var gherkinFileScope = gherkinListener.GetResult();

            var result = new GherkinFileScopeChange(
                gherkinFileScope,
                true, true,
                gherkinFileScope.GetAllBlocks(),
                Enumerable.Empty<IGherkinFileBlock>());

            stopwatch.Stop();
            TraceFinishParse(stopwatch, "full", result);

            return result;
        }

        private GherkinFileScopeChange PartialParse(GherkinTextBufferChange change, IGherkinFileScope previousScope)
        {
            visualStudioTracer.Trace("Start incremental parsing", ParserTraceCategory);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            partialParseCount++;

            var textSnapshot = change.ResultTextSnapshot;

            IScenarioBlock firstAffectedScenario = GetFirstAffectedScenario(change, previousScope);
            VisualStudioTracer.Assert(firstAffectedScenario != null, "first affected scenario is null");
            int parseStartPosition = textSnapshot.GetLineFromLineNumber(firstAffectedScenario.GetStartLine()).Start;

            string fileContent = textSnapshot.GetText(parseStartPosition, textSnapshot.Length - parseStartPosition);

            var gherkinListener = new GherkinTextBufferPartialParserListener(
                previousScope.GherkinDialect,
                textSnapshot, projectScope, 
                previousScope, 
                change.EndLine, change.LineCountDelta);

            var scanner = new GherkinScanner(previousScope.GherkinDialect, fileContent, firstAffectedScenario.GetStartLine());

            IScenarioBlock firstUnchangedScenario = null;
            try
            {
                scanner.Scan(gherkinListener);
            }
            catch (PartialListeningDoneException partialListeningDoneException)
            {
                firstUnchangedScenario = partialListeningDoneException.FirstUnchangedScenario;
            }

            var partialResult = gherkinListener.GetResult();

            var result = MergePartialResult(previousScope, partialResult, firstAffectedScenario, firstUnchangedScenario, change.LineCountDelta);
            stopwatch.Stop();
            TraceFinishParse(stopwatch, "incremental", result);
            return result;
        }

        private IScenarioBlock GetFirstAffectedScenario(GherkinTextBufferChange change, IGherkinFileScope previousScope)
        {
            if (change.Type == GherkinTextBufferChangeType.SingleLine)
                //single-line changes on the start cannot influence the previous scenario
                return previousScope.ScenarioBlocks.LastOrDefault(s => s.GetStartLine() <= change.StartLine);

            // if multiple lines are added at the first line of a block, it can happen that these lines will belong
            // to the previous block
            return previousScope.ScenarioBlocks.LastOrDefault(s => s.GetStartLine() < change.StartLine); 
        }

        private GherkinFileScopeChange MergePartialResult(IGherkinFileScope previousScope, IGherkinFileScope partialResult, IScenarioBlock firstAffectedScenario, IScenarioBlock firstUnchangedScenario, int lineCountDelta)
        {
            Debug.Assert(partialResult.HeaderBlock == null, "Partial parse cannot re-parse header");
            Debug.Assert(partialResult.BackgroundBlock == null, "Partial parse cannot re-parse background");

            List<IGherkinFileBlock> changedBlocks = new List<IGherkinFileBlock>();
            List<IGherkinFileBlock> shiftedBlocks = new List<IGherkinFileBlock>();

            GherkinFileScope fileScope = new GherkinFileScope(previousScope.GherkinDialect, partialResult.TextSnapshot)
                                             {
                                                 HeaderBlock = previousScope.HeaderBlock,
                                                 BackgroundBlock = previousScope.BackgroundBlock
                                             };

            // inserting the non-affected scenarios
            fileScope.ScenarioBlocks.AddRange(previousScope.ScenarioBlocks.TakeUntilItemExclusive(firstAffectedScenario));

            //inserting partial result
            fileScope.ScenarioBlocks.AddRange(partialResult.ScenarioBlocks);
            changedBlocks.AddRange(partialResult.ScenarioBlocks);
            if (partialResult.InvalidFileEndingBlock != null)
            {
                VisualStudioTracer.Assert(firstUnchangedScenario == null, "first affected scenario is not null");
                // the last scenario was changed, but it became invalid
                fileScope.InvalidFileEndingBlock = partialResult.InvalidFileEndingBlock;
                changedBlocks.Add(fileScope.InvalidFileEndingBlock);
            }

            if (firstUnchangedScenario != null)
            {
                Tracing.VisualStudioTracer.Assert(partialResult.InvalidFileEndingBlock == null, "there is an invalid file ending block");

                // inserting the non-effected scenarios at the end
                var shiftedScenarioBlocks = previousScope.ScenarioBlocks.SkipFromItemInclusive(firstUnchangedScenario)
                    .Select(scenario => scenario.Shift(lineCountDelta)).ToArray();
                fileScope.ScenarioBlocks.AddRange(shiftedScenarioBlocks);
                shiftedBlocks.AddRange(shiftedScenarioBlocks);

                if (previousScope.InvalidFileEndingBlock != null)
                {
                    fileScope.InvalidFileEndingBlock = previousScope.InvalidFileEndingBlock.Shift(lineCountDelta);
                    shiftedBlocks.Add(fileScope.InvalidFileEndingBlock);
                }
            }

            return new GherkinFileScopeChange(fileScope, false, false, changedBlocks, shiftedBlocks);
        }

        private void TraceFinishParse(Stopwatch stopwatch, string parseKind, GherkinFileScopeChange result)
        {
            visualStudioTracer.Trace(
                string.Format("Finished {0} parsing in {1} ms, {2} errors", parseKind, stopwatch.ElapsedMilliseconds, result.GherkinFileScope.TotalErrorCount()), ParserTraceCategory);
        }
    }
}
