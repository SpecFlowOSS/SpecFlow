using System;
using System.Linq;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows.Threading;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Classification;
using TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public interface IProjectScopeFactory
    {
        IProjectScope GetProjectScope(Project project);
    }

    [Export(typeof(IProjectScopeFactory))]
    public class ProjectScopeFactory : IDisposable, IProjectScopeFactory
    {
        [Import]
        internal SVsServiceProvider ServiceProvider = null;

        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry = null;

        [Import]
        internal IVisualStudioTracer VisualStudioTracer = null;

        private readonly SynchInitializedInstance<DteWithEvents> dteReference;
        private readonly SynchInitializedInstance<GherkinFileEditorClassifications> classificationsReference;
        private readonly SynchronizedResultCache<Project, string, IProjectScope> projectScopeCache;

        public ProjectScopeFactory()
        {
            dteReference = new SynchInitializedInstance<DteWithEvents>(
                () =>
                    {

                        var dtex = new DteWithEvents(VsxHelper.GetDte(ServiceProvider));
//                        dtex.Events.SolutionEvents.Opened += () => VisualStudioTracer.Trace("solution opened", "ProjectScopeFactory");
                        dtex.SolutionEvents.AfterClosing += OnSolutionClosed;
                        VisualStudioTracer.Trace("subscribed to solution closed " + System.Threading.Thread.CurrentThread.ManagedThreadId, "ProjectScopeFactory");
                        return dtex;
                    });

            classificationsReference = new SynchInitializedInstance<GherkinFileEditorClassifications>(
                () => new GherkinFileEditorClassifications(ClassificationRegistry));

            projectScopeCache = new SynchronizedResultCache<Project, string, IProjectScope>(
                        project => new VsProjectScope(project, dteReference.Value, classificationsReference.Value, VisualStudioTracer),
                        project => project.UniqueName); //TODO: get ID
        }

        public IProjectScope GetProjectScope(Project project)
        {
            if (project == null)
                return NoProjectScope.Instance;

            return projectScopeCache.GetOrCreate(project);
        }

        private void OnSolutionClosed()
        {
            VisualStudioTracer.Trace("solution closed", "ProjectScopeFactory");

            var projectScopes = projectScopeCache.Values.ToArray();
            projectScopeCache.Clear();
            foreach (var projectScope in projectScopes)
            {
                projectScope.Dispose();
            }
        }

        public void Dispose()
        {
            if (dteReference.IsInitialized)
                dteReference.Value.SolutionEvents.AfterClosing -= OnSolutionClosed;
            OnSolutionClosed();
        }
    }
}