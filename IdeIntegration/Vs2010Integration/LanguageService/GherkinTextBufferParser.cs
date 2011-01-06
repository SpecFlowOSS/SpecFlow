using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.Text;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.Gherkin;
using TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public class GherkinTextBufferParser
    {
        private const int PartialParseCountLimit = 30;

        private int partialParseCount = 0;
        private readonly IProjectScope projectScope;

        public GherkinTextBufferParser(IProjectScope projectScope)
        {
            this.projectScope = projectScope;
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
            else if (!previousScope.ScenarioBlocks.Any(s => s.GetStartLine() <= change.StartLine))
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

            return new GherkinFileScopeChange(fileScope, true, true, fileScope.GetAllBlocks(), Enumerable.Empty<IGherkinFileBlock>());
        }

        private GherkinFileScopeChange FullParse(ITextSnapshot textSnapshot, GherkinDialect gherkinDialect)
        {
            partialParseCount = 0;

            var gherkinListener = new GherkinTextBufferParserListener(gherkinDialect, textSnapshot, projectScope.Classifications);

            var scanner = new GherkinScanner(gherkinDialect, textSnapshot.GetText(), 0);
            scanner.Scan(gherkinListener);

            var gherkinFileScope = gherkinListener.GetResult();

            return new GherkinFileScopeChange(
                gherkinFileScope,
                true, true,
                gherkinFileScope.GetAllBlocks(),
                Enumerable.Empty<IGherkinFileBlock>());
        }

        private GherkinFileScopeChange PartialParse(GherkinTextBufferChange change, IGherkinFileScope previousScope)
        {
            partialParseCount++;

            var textSnapshot = change.ResultTextSnapshot;

            IScenarioBlock firstAffectedScenario = GetFirstAffectedScenario(change, previousScope);
            int parseStartPosition = textSnapshot.GetLineFromLineNumber(firstAffectedScenario.GetStartLine()).Start;

            string fileContent = textSnapshot.GetText(parseStartPosition, textSnapshot.Length - parseStartPosition);

            var gherkinListener = new GherkinTextBufferPartialParserListener(
                previousScope.GherkinDialect,
                textSnapshot, projectScope.Classifications, 
                previousScope, 
                change.EndLine, change.LineCountDelta);

            var scanner = new GherkinScanner(previousScope.GherkinDialect, fileContent, firstAffectedScenario.GetStartLine());

            IScenarioBlock firstUnchangedScenario = null;
            try
            {
                scanner.Scan(gherkinListener);
            }
            catch (PartialListeningDone2Exception partialListeningDoneException)
            {
                firstUnchangedScenario = partialListeningDoneException.FirstUnchangedScenario;
            }

            var partialResult = gherkinListener.GetResult();

            return MergePartialResult(previousScope, partialResult, firstAffectedScenario, firstUnchangedScenario, change.LineCountDelta);
        }

        private IScenarioBlock GetFirstAffectedScenario(GherkinTextBufferChange change, IGherkinFileScope previousScope)
        {
            var firstAffectedScenario = previousScope.ScenarioBlocks.LastOrDefault(
                s => s.GetStartLine() <= change.StartLine);
            Debug.Assert(firstAffectedScenario != null);
            return firstAffectedScenario;
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
                Debug.Assert(firstUnchangedScenario == null);
                // the last scenario was changed, but it became invalid
                fileScope.InvalidFileEndingBlock = partialResult.InvalidFileEndingBlock;
                changedBlocks.Add(fileScope.InvalidFileEndingBlock);
            }

            if (firstUnchangedScenario != null)
            {
                Debug.Assert(partialResult.InvalidFileEndingBlock == null);

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
    }
}