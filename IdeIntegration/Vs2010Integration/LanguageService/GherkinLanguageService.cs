using System;
using System.Linq;
using Microsoft.VisualStudio.Text;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    /// <summary>
    /// Class controlling all Gherkin (feature file) related operation in the Visual Studio editor for a given file.
    /// </summary>
    public class GherkinLanguageService
    {
        private readonly IProjectScope projectScope;

        private IGherkinFileScope lastGherkinFileScope = null;

        public IProjectScope ProjectScope
        {
            get { return projectScope; }
        }

        public GherkinLanguageService(IProjectScope projectScope)
        {
            this.projectScope = projectScope;
            AnalyzingEnabled = true;
        }

        public bool AnalyzingEnabled { get; set; }

        /// <summary>
        /// Notifies the subscribers about a change in the (parsed) file scope.
        /// </summary>
        public event EventHandler<GherkinFileScopeChange> FileScopeChanged;

        /// <summary>
        /// Registers a change in the text buffer for the language service. The processing of the change is asynchronous, so it does not block the caller.
        /// </summary>
        public void TextBufferChanged(GherkinTextBufferChange change)
        {
            var task = new ParsingTask(this, change);

            projectScope.GherkinProcessingScheduler.EnqueueParsingRequest(task);
//            task.Apply(); // synchronous execution
        }

        private class ParsingTask : IGherkinProcessingTask
        {
            private readonly GherkinLanguageService languageService;
            private readonly GherkinTextBufferChange change;

            public ParsingTask(GherkinLanguageService languageService, GherkinTextBufferChange change)
            {
                this.languageService = languageService;
                this.change = change;
            }

            public void Apply()
            {
                var lastGherkinFileScope = languageService.GetFileScope();
                var scopeChange = languageService.ProjectScope.GherkinTextBufferParser.Parse(change, lastGherkinFileScope);

                languageService.RegisterScopeChange(scopeChange);
                languageService.EnqueueAnalyzingRequest(scopeChange);
            }

            public IGherkinProcessingTask Merge(IGherkinProcessingTask other)
            {
                if (other is PingTask)
                    return this;

                ParsingTask otherParsingTask = other as ParsingTask;
                if (otherParsingTask == null || languageService != otherParsingTask.languageService)
                    return null;

                return new ParsingTask(
                    languageService, GherkinTextBufferChange.Merge(change, otherParsingTask.change));
            }
        }

        private void RegisterScopeChange(GherkinFileScopeChange scopeChange)
        {
            lastGherkinFileScope = scopeChange.GherkinFileScope;
            TriggerScopeChange(scopeChange);
        }

        private class AnalyzingTask : IGherkinProcessingTask
        {
            private readonly GherkinLanguageService languageService;
            private readonly GherkinFileScopeChange change;

            public AnalyzingTask(GherkinLanguageService languageService, GherkinFileScopeChange change)
            {
                this.languageService = languageService;
                this.change = change;
            }

            public void Apply()
            {
                var newScopeChange = languageService.ProjectScope.GherkinScopeAnalyzer.Analyze(change);
                if (newScopeChange != change)
                    languageService.TriggerScopeChange(newScopeChange);
            }

            public IGherkinProcessingTask Merge(IGherkinProcessingTask other)
            {
                if (other is PingTask)
                    return this;

                AnalyzingTask otherAnalyzingTask = other as AnalyzingTask;
                if (otherAnalyzingTask == null || languageService != otherAnalyzingTask.languageService)
                    return null;

                if (otherAnalyzingTask.change.EntireScopeChanged)
                    return otherAnalyzingTask;

                if (change.EntireScopeChanged)
                    return new AnalyzingTask(languageService, GherkinFileScopeChange.CreateEntireScopeChange(otherAnalyzingTask.change.GherkinFileScope));

                return new AnalyzingTask(languageService, Merge(change, otherAnalyzingTask.change));
            }

            private static GherkinFileScopeChange Merge(GherkinFileScopeChange change1, GherkinFileScopeChange change2)
            {
                var ramainingChanged1Blocks = change1.ChangedBlocks.Intersect(change2.GherkinFileScope.GetAllBlocks()).ToArray();

                var firstChanged1 = ramainingChanged1Blocks.FirstOrDefault();
                var lastChanged1 = ramainingChanged1Blocks.LastOrDefault();

                var firstChanged2 = change2.ChangedBlocks.First();
                var lastChanged2 = change2.ChangedBlocks.Last();

                var firstChanged = firstChanged1.GetStartLine() < firstChanged2.GetStartLine() ? firstChanged1 : firstChanged2;
                var lastChanged = lastChanged1.GetEndLine() > lastChanged2.GetEndLine() ? lastChanged1 : lastChanged2;

                var changedBlocks = change2.GherkinFileScope.GetAllBlocks().SkipFromItemInclusive(firstChanged).TakeUntilItemInclusive(lastChanged);
                var shiftedBlocks = change1.ShiftedBlocks.Any() || change2.ShiftedBlocks.Any() ?
                    change2.GherkinFileScope.GetAllBlocks().SkipFromItemExclusive(lastChanged) :
                    Enumerable.Empty<IGherkinFileBlock>();
                return new GherkinFileScopeChange(change2.GherkinFileScope, false, false, changedBlocks, shiftedBlocks);
            }
        }

        private void EnqueueAnalyzingRequest(GherkinFileScopeChange scopeChange)
        {
            if (!AnalyzingEnabled)
                return;

            var task = new AnalyzingTask(this, scopeChange);

            projectScope.GherkinProcessingScheduler.EnqueueAnalyzingRequest(task);
            //            task.Apply(); // synchronous execution
        }

        private void TriggerScopeChange(GherkinFileScopeChange scopeChange)
        {
            if (FileScopeChanged != null)
                FileScopeChanged(this, scopeChange);
        }

        /// <summary>
        /// Receives a parsed file scope for the buffer.
        /// </summary>
        /// <param name="waitForLatest">If true, the caller is blocked until the most recent scope is produced.</param>
        /// <returns>The parsed file scope.</returns>
        public IGherkinFileScope GetFileScope(bool waitForLatest = false, ITextSnapshot waitForParsingSnapshot = null)
        {
            if (lastGherkinFileScope == null)
                waitForLatest = true;

            //TODO: handle waitForLatest
            return lastGherkinFileScope;
        }
    }
}
