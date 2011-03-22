using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Vs2010Integration.Bindings;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;

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
                var step = GetCurrentStep(triggerPoint.Value);
                if (step == null)
                    return;

                IEnumerable<Completion> completions;
                IEnumerable<Completion> completionBuilders;
                GetCompletionsForBindingType(step.BindingType, out completions, out completionBuilders);

                ITrackingSpan applicableTo = GetApplicableToForStep(snapshot, triggerPoint.Value);

                string displayName = string.Format("All {0} Steps", step);
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

        static internal bool IsStepLine(SnapshotPoint triggerPoint, GherkinLanguageService languageService)
        {
            var line = triggerPoint.GetContainingLine();
            SnapshotPoint start = line.Start;
            ForwardWhile(ref start, triggerPoint, p => char.IsWhiteSpace(p.GetChar()));
            SnapshotPoint end = start;
            ForwardWhile(ref end, triggerPoint, p => !char.IsWhiteSpace(p.GetChar()));
            if (start >= end)
                return false;

            var firstWord = triggerPoint.Snapshot.GetText(start, end.Position - start);
            GherkinDialect dialect = GetDialect(languageService);
            return dialect.IsStepKeyword(firstWord);
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
            if (end < triggerPoint)
                return false;

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

        private ITrackingSpan GetApplicableToForStep(ITextSnapshot snapshot, SnapshotPoint triggerPoint)
        {
            var line = triggerPoint.GetContainingLine();

            SnapshotPoint start = line.Start;
            ForwardWhile(ref start, triggerPoint, p => char.IsWhiteSpace(p.GetChar()));
            ForwardWhile(ref start, triggerPoint, p => !char.IsWhiteSpace(p.GetChar()));
            if (start < triggerPoint)
                ForwardWhile(ref start, start + 1, p => char.IsWhiteSpace(p.GetChar()));

            return snapshot.CreateTrackingSpan(new SnapshotSpan(start, line.End), SpanTrackingMode.EdgeInclusive);
        }

        private static void ForwardWhile(ref SnapshotPoint point, SnapshotPoint triggerPoint, Predicate<SnapshotPoint> predicate)
        {
            while (point < triggerPoint && predicate(point))
                point += 1;
        }

        private GherkinStep GetCurrentStep(SnapshotPoint triggerPoint)
        {
            var fileScope = languageService.GetFileScope(waitForParsingSnapshot: triggerPoint.Snapshot);
            if (fileScope == null)
                return null;

            var triggerLineNumber = triggerPoint.Snapshot.GetLineNumberFromPosition(triggerPoint.Position);
            return fileScope.GetStepAtPosition(triggerLineNumber);
        }

        private void GetCompletionsForBindingType(BindingType bindingType, out IEnumerable<Completion> completions, out IEnumerable<Completion> completionBuilders)
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
                completions = suggestionProvider.GetNativeSuggestionItems(bindingType);
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

