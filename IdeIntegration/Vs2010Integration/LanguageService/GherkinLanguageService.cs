using System;
using Microsoft.VisualStudio.Text;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    /// <summary>
    /// Class controlling all Gherkin (feature file) related operation in the Visual Studio editor for a given file.
    /// </summary>
    public class GherkinLanguageService
    {
        private IProjectScope projectScope;
        private ParsingScheduler parsingScheduler;

        private IGherkinFileScope lastGherkinFileScope = null;

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
            //TODO: parsingScheduler.EnqueueParsingRequest(change);
            var scopeChange = projectScope.GherkinTextBufferParser.Parse(change, lastGherkinFileScope);
            lastGherkinFileScope = scopeChange.GherkinFileScope;

            TriggerScopeChange(scopeChange);

            if (AnalyzingEnabled)
                EnqueueAnalyzingRequest(scopeChange);
        }

        private void EnqueueAnalyzingRequest(GherkinFileScopeChange scopeChange)
        {
            //TODO: parsingScheduler.EnqueueAnalyzingRequest(change);
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
        public IGherkinFileScope GetFileScope(bool waitForLatest = false)
        {
            if (lastGherkinFileScope == null)
                waitForLatest = true;

            //TODO: handle waitForLatest
            return lastGherkinFileScope;
        }
    }
}
