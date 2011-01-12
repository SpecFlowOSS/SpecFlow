using System;
using Microsoft.VisualStudio.Text;

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

        private void EnqueueAnalyzingRequest(GherkinFileScopeChange scopeChange)
        {
            if (!AnalyzingEnabled)
                return;

            //TODO: GherkinProcessingScheduler.EnqueueAnalyzingRequest(change);
            var newScopeChange = projectScope.GherkinScopeAnalyzer.Analyze(scopeChange, null); //TODO: specify previous scope
            if (newScopeChange != scopeChange)
                TriggerScopeChange(newScopeChange);
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
