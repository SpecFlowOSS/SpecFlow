using System;
using System.ComponentModel.Composition;
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

        private readonly SynchInitializedInstance<DTE> dteReference;
        private readonly SynchInitializedInstance<GherkinFileEditorClassifications> classificationsReference;
        private readonly SynchronizedResultCache<Project, string, IProjectScope> projectScopeCache;

        public ProjectScopeFactory()
        {
            dteReference = new SynchInitializedInstance<DTE>(
                () =>
                    {
                        var dtex = VsxHelper.GetDte(ServiceProvider);
                        dtex.Events.SolutionEvents.AfterClosing += OnSolutionClosed;
                        return dtex;
                    });

            classificationsReference = new SynchInitializedInstance<GherkinFileEditorClassifications>(
                () => new GherkinFileEditorClassifications(ClassificationRegistry));

            projectScopeCache = new SynchronizedResultCache<Project, string, IProjectScope>(
                        project =>
                            {
                                dteReference.EnsureInitialized();
                                return new VsProjectScope(project, classificationsReference.Value, VisualStudioTracer);
                            },
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
            projectScopeCache.Clear();
        }

        public void Dispose()
        {
            if (dteReference.IsInitialized)
                dteReference.Value.Events.SolutionEvents.AfterClosing -= OnSolutionClosed;
        }
    }
}