using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.Text;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    /// <summary>
    /// Class controlling all Gherkin (feature file) related operation in the Visual Studio editor for a given file.
    /// </summary>
    public class GherkinLanguageService : IDisposable
    {
        private readonly IProjectScope projectScope;
        private readonly IVisualStudioTracer visualStudioTracer;
        private readonly bool enableStepMatchColoring;

        private bool isDisposed = false;
        private IGherkinFileScope lastGherkinFileScope = null;
        private ITextSnapshot lastRegisteredSnapshot = null;

        private bool isInitialized = false;

        public IProjectScope ProjectScope
        {
            get { return projectScope; }
        }

        public GherkinLanguageService(IProjectScope projectScope, IVisualStudioTracer visualStudioTracer, bool enableStepMatchColoring)
        {
            this.projectScope = projectScope;
            this.visualStudioTracer = visualStudioTracer;
            this.enableStepMatchColoring = enableStepMatchColoring && projectScope.StepSuggestionProvider != null;
            AnalyzingEnabled = projectScope.GherkinScopeAnalyzer != null;

            visualStudioTracer.Trace("Language service created", "GherkinLanguageService");
        }

        public void EnsureInitialized(ITextBuffer textBuffer)
        {
            if (!isInitialized)
            {
                lock(this)
                {
                    if (!isInitialized)
                    {
                        Initialize(textBuffer);
                        isInitialized = true;
                    }
                }
            }
        }

        private void Initialize(ITextBuffer textBuffer)
        {
            // do initial parsing
            TextBufferChanged(GherkinTextBufferChange.CreateEntireBufferChange(textBuffer.CurrentSnapshot));

            projectScope.GherkinDialectServicesChanged += ReParseEntireFile;

            if (enableStepMatchColoring)
            {
                projectScope.StepSuggestionProvider.Ready += ReParseEntireFile;
                projectScope.StepSuggestionProvider.BindingsChanged += ReParseEntireFile;
            }
        }

        private void ReParseEntireFile()
        {
            if (lastGherkinFileScope == null || lastGherkinFileScope.TextSnapshot == null)
                return;

            TextBufferChanged(GherkinTextBufferChange.CreateEntireBufferChange(lastGherkinFileScope.TextSnapshot.TextBuffer.CurrentSnapshot));
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
            if (isDisposed)
                throw new ObjectDisposedException("GherkinLanguageService");

            lastRegisteredSnapshot = change.ResultTextSnapshot;
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
                if (languageService.isDisposed)
                    return;

                var lastGherkinFileScope = languageService.GetFileScope(waitForResult: false);
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

            public override string ToString()
            {
                return string.Format("Parse Gherkin: {0}", change.Type);
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
                if (languageService.isDisposed)
                    return;

                var newScopeChange = languageService.ProjectScope.GherkinScopeAnalyzer.Analyze(change);
                if (newScopeChange != change)
                    languageService.TriggerScopeChange(newScopeChange);
            }

            public IGherkinProcessingTask Merge(IGherkinProcessingTask other)
            {
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

                var firstChanged = firstChanged1 != null && firstChanged1.GetStartLine() < firstChanged2.GetStartLine() ? firstChanged1 : firstChanged2;
                var lastChanged = lastChanged1 != null && lastChanged1.GetEndLine() > lastChanged2.GetEndLine() ? lastChanged1 : lastChanged2;

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
        /// <param name="waitForParsingSnapshot"></param>
        /// <returns>The parsed file scope.</returns>
        public IGherkinFileScope GetFileScope(bool waitForLatest = false, ITextSnapshot waitForParsingSnapshot = null, bool waitForResult = true)
        {
            if (isDisposed)
                throw new ObjectDisposedException("GherkinLanguageService");

            if (!waitForResult && lastGherkinFileScope == null)
                return null;

            if (lastGherkinFileScope == null)
                waitForLatest = true;

            if (waitForLatest)
            {
                while (lastRegisteredSnapshot == null)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(50));
                }
                waitForParsingSnapshot = lastRegisteredSnapshot;
            }

            if (waitForParsingSnapshot != null)
            {
                int counter = 0;
                while (counter < 10 && 
                    (lastGherkinFileScope == null || lastGherkinFileScope.TextSnapshot != waitForParsingSnapshot))
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(50));
                    counter++;
                }
            }

            return lastGherkinFileScope;
        }

        public void Dispose()
        {
            visualStudioTracer.Trace("Language service disposed", "GherkinLanguageService");
            isDisposed = true;
            projectScope.GherkinDialectServicesChanged -= ReParseEntireFile;
            if (enableStepMatchColoring)
            {
                projectScope.StepSuggestionProvider.Ready -= ReParseEntireFile;
                projectScope.StepSuggestionProvider.BindingsChanged -= ReParseEntireFile;
            }
            lastGherkinFileScope = null;
        }
    }
}
