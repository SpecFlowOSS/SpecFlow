using System;
using System.ComponentModel.Composition;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

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

        private readonly SynchInitializedInstance<DTE> dteReference;
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

            projectScopeCache = new SynchronizedResultCache<Project, string, IProjectScope>(
                        project =>
                            {
                                dteReference.EnsureInitialized();
                                return new VsProjectScope(project);
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