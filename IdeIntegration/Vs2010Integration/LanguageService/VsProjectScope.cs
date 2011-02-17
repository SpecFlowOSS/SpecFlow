using System;
using EnvDTE;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public class VsProjectScope : IProjectScope
    {
        private readonly Project project;
        private readonly IVisualStudioTracer visualStudioTracer;
        private readonly GherkinTextBufferParser parser;
        private readonly GherkinScopeAnalyzer analyzer;
        private readonly SynchInitializedInstance<SpecFlowProjectConfiguration> specFlowProjectConfigurationReference;
        private readonly SynchInitializedInstance<GherkinDialectServices> gherkinDialectServicesReference;

        private readonly VsProjectFileTracker appConfigTracker;

        public GherkinFileEditorClassifications Classifications { get; private set; }
        public GherkinProcessingScheduler GherkinProcessingScheduler { get; private set; }
        public SpecFlowProjectConfiguration SpecFlowProjectConfiguration
        {
            get { return specFlowProjectConfigurationReference.Value; }
            set { specFlowProjectConfigurationReference.Value = value; }
        }

        public GherkinDialectServices GherkinDialectServices
        {
            get { return gherkinDialectServicesReference.Value; }
            set { gherkinDialectServicesReference.Value = value; }
        }

        public Project Project
        {
            get { return project; }
        }

        public IVisualStudioTracer VisualStudioTracer
        {
            get { return visualStudioTracer; }
        }

        public event EventHandler SpecFlowProjectConfigurationChanged;
        public event EventHandler GherkinDialectServicesChanged;

        internal VsProjectScope(Project project, DteWithEvents dteWithEvents, GherkinFileEditorClassifications classifications, IVisualStudioTracer visualStudioTracer)
        {
            Classifications = classifications;
            this.project = project;
            this.visualStudioTracer = visualStudioTracer;
            //TODO: register for file changes, etc.

            parser = new GherkinTextBufferParser(this, visualStudioTracer);
            analyzer = new GherkinScopeAnalyzer(this, visualStudioTracer);
            GherkinProcessingScheduler = new GherkinProcessingScheduler(visualStudioTracer);

            specFlowProjectConfigurationReference = new SynchInitializedInstance<SpecFlowProjectConfiguration>(()=>
                DteProjectReader.LoadSpecFlowConfigurationFromDteProject(project) ?? new SpecFlowProjectConfiguration());
            gherkinDialectServicesReference = new SynchInitializedInstance<GherkinDialectServices>(() =>
                new GherkinDialectServices(SpecFlowProjectConfiguration.GeneratorConfiguration.FeatureLanguage));

            appConfigTracker = new VsProjectFileTracker(project, "App.config", dteWithEvents, visualStudioTracer);
            appConfigTracker.FileChanged += AppConfigTrackerOnFileChanged;
        }

        private void AppConfigTrackerOnFileChanged(ProjectItem appConfigItem)
        {
            var newConfig = DteProjectReader.LoadSpecFlowConfigurationFromDteProject(project) ?? new SpecFlowProjectConfiguration();
            if (newConfig.Equals(SpecFlowProjectConfiguration)) 
                return;

            bool dialectServicesChanged = !newConfig.GeneratorConfiguration.FeatureLanguage.Equals(GherkinDialectServices.DefaultLanguage);

            SpecFlowProjectConfiguration = newConfig;
            this.visualStudioTracer.Trace("SpecFlow configuration changed", "VsProjectScope");
            if (SpecFlowProjectConfigurationChanged != null)
                SpecFlowProjectConfigurationChanged(this, EventArgs.Empty);

            if (dialectServicesChanged)
            {
                GherkinDialectServices = new GherkinDialectServices(SpecFlowProjectConfiguration.GeneratorConfiguration.FeatureLanguage);
                this.visualStudioTracer.Trace("default language changed", "VsProjectScope");
                if (GherkinDialectServicesChanged != null)
                    GherkinDialectServicesChanged(this, EventArgs.Empty);
            }
        }

        public GherkinTextBufferParser GherkinTextBufferParser
        {
            get { return parser; }
        }

        public GherkinScopeAnalyzer GherkinScopeAnalyzer
        {
            get { return analyzer; }
        }

        public void Dispose()
        {
            GherkinProcessingScheduler.Dispose();
        }
    }
}