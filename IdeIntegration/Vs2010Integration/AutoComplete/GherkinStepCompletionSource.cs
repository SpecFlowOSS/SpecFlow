using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.Gherkin;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using TechTalk.SpecFlow.Vs2010Integration.Options;

namespace TechTalk.SpecFlow.Vs2010Integration.AutoComplete
{
    [Export(typeof(ICompletionSourceProvider))]
    [ContentType("gherkin")]
    [Name("gherkinStepCompletion")]
    internal class GherkinStepCompletionSourceProvider : ICompletionSourceProvider
    {
        [Import]
        IIntegrationOptionsProvider IntegrationOptionsProvider = null;

        [Import]
        IGherkinLanguageServiceFactory GherkinLanguageServiceFactory = null;

        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            if (!IntegrationOptionsProvider.GetOptions().EnableIntelliSense)
                return null;

            return new GherkinStepCompletionSource(textBuffer, GherkinLanguageServiceFactory.GetLanguageService(textBuffer));
        }
    }

    internal class GherkinStepCompletionSource : ICompletionSource
    {
        private bool disposed = false;
        private readonly ITextBuffer textBuffer;
        private readonly GherkinLanguageService languageService;

        public GherkinStepCompletionSource(ITextBuffer textBuffer, GherkinLanguageService languageService)
        {
            this.textBuffer = textBuffer;
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

            if (IsKeywordCompletion(triggerPoint.Value))
            {
                IEnumerable<Completion> completions = GetKeywordCompletions();
                ITrackingSpan applicableTo = GetApplicableToForKeyword(snapshot, triggerPoint.Value);

                completionSets.Add(
                    new CustomCompletionSet(
                        "Keywords",
                        "Keywords",
                        applicableTo,
                        completions,
                        null));
            }
            else
            {
                string parsedKeyword;
                var bindingType = GetCurrentBindingType(triggerPoint.Value, out parsedKeyword);
                if (bindingType == null)
                    return;

                IEnumerable<Completion> completions;
                IEnumerable<Completion> completionBuilders;
                GetCompletionsForBindingType(bindingType.Value, out completions, out completionBuilders);

                ITrackingSpan applicableTo = GetApplicableToForStep(snapshot, triggerPoint.Value, parsedKeyword);

                string displayName = string.Format("All {0} Steps", bindingType.Value);
                completionSets.Add(
                    new HierarchicalCompletionSet(
                        displayName,
                        displayName,
                        applicableTo,
                        completions,
                        completionBuilders));
            }
        }

        private IEnumerable<Completion> GetKeywordCompletions()
        {
            GherkinDialect dialect = GetDialect(languageService);
            return dialect.GetStepKeywords().Select(k => new Completion(k.Trim(), k.Trim(), null, null, null)).Concat(
                dialect.GetBlockKeywords().Select(k => new Completion(k.Trim(), k.Trim() + ": ", null, null, null)));
        }

        static private GherkinDialect GetDialect(GherkinLanguageService languageService)
        {
            var fileScope = languageService.GetFileScope();
            return fileScope != null ? fileScope.GherkinDialect : languageService.ProjectScope.GherkinDialectServices.GetDefaultDialect();
        }

        static internal bool IsKeywordCompletion(SnapshotPoint triggerPoint)
        {
            var line = triggerPoint.GetContainingLine();
            SnapshotPoint start = line.Start;
            ForwardWhile(ref start, triggerPoint, p => char.IsWhiteSpace(p.GetChar()));
            ForwardWhile(ref start, triggerPoint, p => !char.IsWhiteSpace(p.GetChar()));
            return start == triggerPoint;
        }

        static private string GetFirstWord(SnapshotPoint triggerPoint)
        {
            var line = triggerPoint.GetContainingLine();
            SnapshotPoint start = line.Start;
            ForwardWhile(ref start, triggerPoint, p => char.IsWhiteSpace(p.GetChar()));
            SnapshotPoint end = start;
            ForwardWhile(ref end, triggerPoint, p => !char.IsWhiteSpace(p.GetChar()));
            if (start >= end)
                return null;

            return triggerPoint.Snapshot.GetText(start, end.Position - start);
        }

        //HACK: this is a hotfix to support "Gegeben sei" 'Given' keyword in German
        static private string GetFirstTwoWords(SnapshotPoint triggerPoint)
        {
            var line = triggerPoint.GetContainingLine();
            SnapshotPoint start = line.Start;
            ForwardWhile(ref start, triggerPoint, p => char.IsWhiteSpace(p.GetChar()));
            SnapshotPoint end = start;
            ForwardWhile(ref end, triggerPoint, p => !char.IsWhiteSpace(p.GetChar()));
            ForwardWhile(ref end, triggerPoint, p => char.IsWhiteSpace(p.GetChar()));
            ForwardWhile(ref end, triggerPoint, p => !char.IsWhiteSpace(p.GetChar()));
            if (start >= end)
                return null;

            return triggerPoint.Snapshot.GetText(start, end.Position - start);
        }

        static internal bool IsStepLine(SnapshotPoint triggerPoint, GherkinLanguageService languageService)
        {
            var keywordCandidate = GetFirstWord(triggerPoint);
            if (keywordCandidate == null)
                return false;
            GherkinDialect dialect = GetDialect(languageService);
            if (dialect == null)
                return false;

            if (dialect.IsStepKeyword(keywordCandidate))
                return true;

            keywordCandidate = GetFirstTwoWords(triggerPoint);
            if (keywordCandidate == null)
                return false;
            return dialect.IsStepKeyword(keywordCandidate);
        }

        static internal bool IsKeywordPrefix(SnapshotPoint triggerPoint, GherkinLanguageService languageService)
        {
            var line = triggerPoint.GetContainingLine();
            SnapshotPoint start = line.Start;
            ForwardWhile(ref start, triggerPoint, p => char.IsWhiteSpace(p.GetChar()));
            SnapshotPoint end = start;
            ForwardWhile(ref end, triggerPoint, p => !char.IsWhiteSpace(p.GetChar()));
            if (start >= end)
                return true; // returns true for empty word

            end = triggerPoint;
//            if (end < triggerPoint)
//                return false;

            var firstWord = triggerPoint.Snapshot.GetText(start, end.Position - start);
            GherkinDialect dialect = GetDialect(languageService);
            return dialect.GetKeywords().Any(k => k.StartsWith(firstWord, StringComparison.CurrentCultureIgnoreCase));
        }

        private ITrackingSpan GetApplicableToForKeyword(ITextSnapshot snapshot, SnapshotPoint triggerPoint)
        {
            var line = triggerPoint.GetContainingLine();
            SnapshotPoint start = line.Start;
            ForwardWhile(ref start, triggerPoint, p => char.IsWhiteSpace(p.GetChar()));
            return snapshot.CreateTrackingSpan(new SnapshotSpan(start, line.End), SpanTrackingMode.EdgeInclusive);
        }

        private ITrackingSpan GetApplicableToForStep(ITextSnapshot snapshot, SnapshotPoint triggerPoint, string parsedKeyword)
        {
            var line = triggerPoint.GetContainingLine();

            SnapshotPoint keywordEnd = line.Start;
            ForwardWhile(ref keywordEnd, triggerPoint, p => char.IsWhiteSpace(p.GetChar()));
            if (parsedKeyword != null)
                keywordEnd += parsedKeyword.Length;
            else
                ForwardWhile(ref keywordEnd, triggerPoint, p => !char.IsWhiteSpace(p.GetChar()));

            var start = keywordEnd;
            if (start < triggerPoint)
                ForwardWhile(ref start, start + 1, p => char.IsWhiteSpace(p.GetChar()));

            return snapshot.CreateTrackingSpan(new SnapshotSpan(start, line.End), SpanTrackingMode.EdgeInclusive);
        }

        private static void ForwardWhile(ref SnapshotPoint point, SnapshotPoint triggerPoint, Predicate<SnapshotPoint> predicate)
        {
            while (point < triggerPoint && predicate(point))
                point += 1;
        }

        private StepDefinitionType? GetCurrentBindingType(SnapshotPoint triggerPoint, out string parsedKeyword)
        {
            parsedKeyword = null;
            var fileScope = languageService.GetFileScope(waitForParsingSnapshot: triggerPoint.Snapshot);
            if (fileScope == null)
                return null;

            var triggerLineNumber = triggerPoint.Snapshot.GetLineNumberFromPosition(triggerPoint.Position);
            var step = fileScope.GetStepAtPosition(triggerLineNumber);
            if (step != null)
            {
                parsedKeyword = step.Keyword.TrimEnd();
                return step.StepDefinitionType;
            }

            if (!IsStepLine(triggerPoint, languageService))
                return null;

            // this is a step line that just started. we need to calculate the binding type from
            // the keyword and the context
            var keywordCandidate = GetFirstWord(triggerPoint);
            if (keywordCandidate == null)
                return null;

            GherkinDialect dialect = GetDialect(languageService);
            var stepKeyword = dialect.TryParseStepKeyword(keywordCandidate);
            if (stepKeyword == null)
            {
                keywordCandidate = GetFirstTwoWords(triggerPoint);
                if (keywordCandidate != null)
                {
                    stepKeyword = dialect.TryParseStepKeyword(keywordCandidate);
                }

                if (stepKeyword == null)
                    return null;
            }

            parsedKeyword = keywordCandidate;

            if (stepKeyword == StepKeyword.Given)
                return StepDefinitionType.Given;
            if (stepKeyword == StepKeyword.When)
                return StepDefinitionType.When;
            if (stepKeyword == StepKeyword.Then)
                return StepDefinitionType.Then;

            parsedKeyword = null;
            // now we need the context
            var stepBlock = fileScope.GetStepBlockFromStepPosition(triggerLineNumber);
            var lastStep = stepBlock.Steps.LastOrDefault(s => s.BlockRelativeLine + stepBlock.KeywordLine < triggerLineNumber);
            if (lastStep == null)
                return StepDefinitionType.Given;
            return lastStep.StepDefinitionType;
        }

        private void GetCompletionsForBindingType(StepDefinitionType stepDefinitionType, out IEnumerable<Completion> completions, out IEnumerable<Completion> completionBuilders)
        {
            completionBuilders = null;

            var suggestionProvider = languageService.ProjectScope.StepSuggestionProvider;
            if (suggestionProvider == null)
            {
                completions = Enumerable.Empty<Completion>();
                return;
            }

            if (!suggestionProvider.Populated)
            {
                string percentText = string.Format("({0}% completed)", suggestionProvider.GetPopulationPercent());
                completionBuilders = new[] {new Completion(
                    (!suggestionProvider.BindingsPopulated ? 
                        "step suggestion list is being populated... " : 
                        "step suggestion list from existing feature files is being populated... ") + percentText)};
            }

            try
            {
                completions = suggestionProvider.GetNativeSuggestionItems(stepDefinitionType);
            }
            catch(Exception)
            {
                //fallback case
                completions = Enumerable.Empty<Completion>();
            }
        }

        public void Dispose()
        {
            disposed = true;
        }
    }
}

