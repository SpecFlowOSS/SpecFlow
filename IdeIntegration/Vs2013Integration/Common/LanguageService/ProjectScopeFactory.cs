using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Composition;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Classification;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.IdeIntegration.Install;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor;
using TechTalk.SpecFlow.Vs2010Integration.Options;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.Utils;
using Thread = System.Threading.Thread;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public interface IProjectScopeFactory
    {
        IProjectScope GetProjectScope(Project project);
        IEnumerable<IProjectScope> GetProjectScopesFromBindingProject(Project bindingProject);
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

        [Import]
        internal IIntegrationOptionsProvider IntegrationOptionsProvider = null;

        [Import]
        internal IBiDiContainerProvider ContainerProvider = null;

        private readonly SynchInitializedInstance<DteWithEvents> dteReference;
        private readonly SynchInitializedInstance<GherkinFileEditorClassifications> classificationsReference;
        private readonly SynchronizedResultCache<Project, string, IProjectScope> projectScopeCache;
        private readonly SynchInitializedInstance<NoProjectScope> noProjectScopeReference;


        public ProjectScopeFactory()
        {
            dteReference = new SynchInitializedInstance<DteWithEvents>(
                () =>
                    {
                        ContainerProvider.ObjectContainer.Resolve<InstallServices>().OnPackageUsed(); //TODO: find a better place
                        var dtex = new DteWithEvents(VsxHelper.GetDte(ServiceProvider), VisualStudioTracer);
                        dtex.SolutionEvents.AfterClosing += OnSolutionClosed;
                        dtex.SolutionEventsListener.OnQueryUnloadProject += OnProjectClosed;
                        VisualStudioTracer.Trace("subscribed to solution closed " + Thread.CurrentThread.ManagedThreadId, "ProjectScopeFactory");
                        return dtex;
                    });

            classificationsReference = new SynchInitializedInstance<GherkinFileEditorClassifications>(
                () => new GherkinFileEditorClassifications(ClassificationRegistry));

            projectScopeCache = new SynchronizedResultCache<Project, string, IProjectScope>(
                        project => new VsProjectScope(project, dteReference.Value, classificationsReference.Value, VisualStudioTracer, IntegrationOptionsProvider),
                        VsxHelper.GetProjectUniqueId);

            noProjectScopeReference = new SynchInitializedInstance<NoProjectScope>(() =>
                new NoProjectScope(classificationsReference.Value, VisualStudioTracer, IntegrationOptionsProvider));
        }

        public IProjectScope GetProjectScope(Project project)
        {
            if (project == null || !VsProjectScope.IsProjectSupported(project))
                return noProjectScopeReference.Value;

            return projectScopeCache.GetOrCreate(project);
        }

        public IEnumerable<IProjectScope> GetProjectScopesFromBindingProject(Project bindingProject)
        {
            if (bindingProject == null)
                yield break;

            var scope = projectScopeCache.Get(bindingProject);

            var assemblyName = bindingProject.GetAssemblyName();
            if (assemblyName == null)
                yield break;

            var projectScopes = projectScopeCache.Values.ToArray();
            foreach (var projectScope in projectScopes)
            {
                if (projectScope == scope || 
                    GetAdditionalBindingAssemblyNames(projectScope).Any(an => an.Equals(assemblyName, StringComparison.InvariantCultureIgnoreCase)))

                    yield return projectScope;
            }
        }

        private static IEnumerable<string> GetAdditionalBindingAssemblyNames(IProjectScope projectScope)
        {
            return projectScope.SpecFlowProjectConfiguration.RuntimeConfiguration.AdditionalStepAssemblies.Select(a => a.Split(new[] {','}, 2)[0]);
        }

        private void OnProjectClosed(Project project)
        {
            VisualStudioTracer.Trace("project closed", "ProjectScopeFactory");

            var projectScope = projectScopeCache.Pop(project);
            if (projectScope != null)
                projectScope.Dispose();
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