using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.Gherkin;
using TechTalk.SpecFlow.Utils;
using TechTalk.SpecFlow.Vs2010Integration.Bindings;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using ScenarioBlock = TechTalk.SpecFlow.Parser.Gherkin.ScenarioBlock;

namespace TechTalk.SpecFlow.Vs2010Integration.AutoComplete
{
    [Export(typeof(ICompletionSourceProvider))]
    [ContentType("gherkin")]
    [Name("gherkinStepCompletion")]
    internal class GherkinStepCompletionSourceProvider : ICompletionSourceProvider
    {
        [Import]
        ISpecFlowServices SpecFlowServices = null;

        [Import]
        IGherkinLanguageServiceFactory GherkinLanguageServiceFactory = null;

        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            if (!SpecFlowServices.GetOptions().EnableIntelliSense)
                return null;

            return new GherkinStepCompletionSource(textBuffer, SpecFlowServices, GherkinLanguageServiceFactory.GetLanguageService(textBuffer));
        }
    }

    internal class GherkinStepCompletionSource : ICompletionSource
    {
        private bool disposed = false;
        private readonly ITextBuffer textBuffer;
        private readonly ISpecFlowServices specFlowServices;
        private readonly GherkinLanguageService languageService;

        public GherkinStepCompletionSource(ITextBuffer textBuffer, ISpecFlowServices specFlowServices, GherkinLanguageService languageService)
        {
            this.textBuffer = textBuffer;
            this.specFlowServices = specFlowServices;
            this.languageService = languageService;
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            if (disposed)
                throw new ObjectDisposedException("GherkinStepCompletionSource");

            ITextSnapshot snapshot = textBuffer.CurrentSnapshot;
            var triggerPoint = session.GetTriggerPoint(snapshot);
            if (triggerPoint == null)
                return;

            ScenarioBlock? scenarioBlock = GetCurrentScenarioBlock(triggerPoint.Value);
            if (scenarioBlock == null)
                return;

            IEnumerable<Completion> completions = GetCompletionsForBindingType((BindingType)scenarioBlock.Value);
            ITrackingSpan applicableTo = GetApplicableToSpan(snapshot, triggerPoint.Value);

            string displayName = string.Format("All {0} Steps", scenarioBlock);
            completionSets.Add(
                new HierarchicalCompletionSet(
                    displayName,
                    displayName,
                    applicableTo,
                    completions,
                    null));
        }

        private ITrackingSpan GetApplicableToSpan(ITextSnapshot snapshot, SnapshotPoint triggerPoint)
        {
            var line = triggerPoint.GetContainingLine();

            SnapshotPoint start = triggerPoint;
            while (start > line.Start && !char.IsWhiteSpace((start - 1).GetChar()))
            {
                start -= 1;
            }

            return snapshot.CreateTrackingSpan(new SnapshotSpan(start, line.End), SpanTrackingMode.EdgeInclusive);
        }

        private ScenarioBlock? GetCurrentScenarioBlock(SnapshotPoint triggerPoint)
        {
            var fileScope = languageService.GetFileScope(waitForParsingSnapshot: triggerPoint.Snapshot);
            if (fileScope == null)
                return null;

            var triggerLineNumber = triggerPoint.Snapshot.GetLineNumberFromPosition(triggerPoint.Position);
            var scenarioInfo = fileScope.GetAllBlocks().LastOrDefault(si => si.KeywordLine < triggerLineNumber);
            if (scenarioInfo == null || !(scenarioInfo is IScenarioBlock || scenarioInfo is IBackgroundBlock))
                return null;

            for (var lineNumer = triggerLineNumber; lineNumer > scenarioInfo.KeywordLine; lineNumer--)
            {
                StepKeyword? stepKeyword = GetStepKeyword(triggerPoint.Snapshot, lineNumer, fileScope.GherkinDialect);

                if (stepKeyword != null)
                {
                    var scenarioBlock = stepKeyword.Value.ToScenarioBlock();
                    if (scenarioBlock != null)
                        return scenarioBlock;
                }
            }

            return ScenarioBlock.Given;
        }

        private StepKeyword? GetStepKeyword(ITextSnapshot snapshot, int lineNumer, GherkinDialect gherkinDialect)
        {
            var word = GetFirstWordOfLine(snapshot, lineNumer);
            return gherkinDialect.GetStepKeyword(word);
        }

        private static string GetFirstWordOfLine(ITextSnapshot snapshot, int lineNumer)
        {
            var theLine = snapshot.GetLineFromLineNumber(lineNumer);
            return theLine.GetText().TrimStart().Split(' ')[0];
        }

        private IEnumerable<Completion> GetCompletionsForBindingType(BindingType bindingType)
        {
            var suggestionProvider = languageService.ProjectScope.StepSuggestionProvider;
            if (suggestionProvider == null)
                return Enumerable.Empty<Completion>();

            if (!suggestionProvider.Populated)
                return new Completion[] {new Completion("step suggestion list is being populated...")};

            return suggestionProvider.GetNativeSuggestionItems(bindingType);
        }

        public void Dispose()
        {
            disposed = true;
        }
    }
}

