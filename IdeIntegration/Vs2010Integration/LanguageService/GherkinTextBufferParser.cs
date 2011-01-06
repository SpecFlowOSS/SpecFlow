using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using gherkin;
using Microsoft.VisualStudio.Text;
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

        private I18n GetLanguageService(ITextSnapshot textSnapshot)
        {
            return projectScope.GherkinLanguageServices.GetLanguageService(lineNo => textSnapshot.GetLineFromLineNumber(lineNo).GetText());
        }

        public GherkinFileScopeChange Parse(GherkinTextBufferChange change, IGherkinFileScope previousScope = null)
        {
            I18n languageService = GetLanguageService(change.ResultTextSnapshot);

            bool fullParse = false;
            if (previousScope == null)
                fullParse = true;
            else if (previousScope.LanguageService != languageService)
                fullParse = true;
            else if (partialParseCount >= PartialParseCountLimit)
                fullParse = true;
            else if (!previousScope.ScenarioBlocks.Any(s => s.GetStartLine() <= change.StartLine))
                fullParse = true;

            if (fullParse)
                return FullParse(change.ResultTextSnapshot, languageService);

            return PartialParse(change, previousScope);
        }

        private GherkinFileScopeChange FullParse(ITextSnapshot textSnapshot, I18n languageService)
        {
            partialParseCount = 0;

            var gherkinListener = new GherkinTextBufferParserListener(textSnapshot, projectScope.Classifications);

            var scanner = new GherkinScanner(languageService, textSnapshot.GetText(), 0);
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
                textSnapshot, projectScope.Classifications, 
                previousScope, 
                change.EndLine, change.LineCountDelta);

            var scanner = new GherkinScanner(previousScope.LanguageService, fileContent, firstAffectedScenario.GetStartLine());

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

            GherkinFileScope fileScope = new GherkinFileScope
                                             {
                                                 HeaderBlock = previousScope.HeaderBlock,
                                                 BackgroundBlock = previousScope.BackgroundBlock
                                             };

            // inserting the non-affected scenarios
            fileScope.ScenarioBlocks.AddRange(previousScope.ScenarioBlocks.TakeUntilItemExclusive(firstAffectedScenario));

            //inserting partial result
            fileScope.ScenarioBlocks.AddRange(partialResult.ScenarioBlocks);

            List<IScenarioBlock> shiftedBlocks = new List<IScenarioBlock>();
//            IScenarioBlock firstUnchangedScenarioShifted = null;
            if (firstUnchangedScenario != null)
            {
                // inserting the non-effected scenarios at the end

                int firstNewScenarioIndex = fileScope.ScenarioBlocks.Count;
                shiftedBlocks.AddRange(
                    previousScope.ScenarioBlocks.SkipFromItemInclusive(firstUnchangedScenario)
                        .Select(scenario => scenario.Shift(lineCountDelta)));
                fileScope.ScenarioBlocks.AddRange(shiftedBlocks);

//                firstUnchangedScenarioShifted = fileScope.ScenarioBlocks.Count > firstNewScenarioIndex
//                                                    ? fileScope.ScenarioBlocks[firstNewScenarioIndex]
//                                                    : null;
            }

            return new GherkinFileScopeChange(fileScope, false, false, partialResult.ScenarioBlocks, shiftedBlocks);
        }
    }
}