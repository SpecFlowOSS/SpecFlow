using System;
using System.Linq;
using System.Windows.Forms;
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
        private readonly DteWithEvents dteWithEvents;
        private readonly IVisualStudioTracer visualStudioTracer;
        private readonly GherkinTextBufferParser parser;
        private readonly GherkinScopeAnalyzer analyzer;
        public GherkinFileEditorClassifications Classifications { get; private set; }
        public GherkinProcessingScheduler GherkinProcessingScheduler { get; private set; }

        private bool initialized = false;
        
        // delay initialized members
        private SpecFlowProjectConfiguration specFlowProjectConfiguration = null;
        private GherkinDialectServices gherkinDialectServices = null;
        private VsProjectFileTracker appConfigTracker = null;
        private ProjectFeatureFilesTracker featureFilesTracker = null;
        private ProjectStepSuggestionProvider projectStepSuggestionProvider = null;

        public SpecFlowProjectConfiguration SpecFlowProjectConfiguration
        {
            get
            {
                EnsureInitialized();
                return specFlowProjectConfiguration;
            }
        }

        public GherkinDialectServices GherkinDialectServices
        {
            get
            {
                EnsureInitialized();
                return gherkinDialectServices;
            }
        }

        internal ProjectFeatureFilesTracker FeatureFilesTracker
        {
            get
            {
                EnsureInitialized();
                return featureFilesTracker;
            }
        }

        public Project Project { get { return project; } }
        public IVisualStudioTracer VisualStudioTracer { get { return visualStudioTracer; } }
        internal DteWithEvents DteWithEvents { get { return dteWithEvents; } }

        public event EventHandler SpecFlowProjectConfigurationChanged;
        public event EventHandler GherkinDialectServicesChanged;

        internal VsProjectScope(Project project, DteWithEvents dteWithEvents, GherkinFileEditorClassifications classifications, IVisualStudioTracer visualStudioTracer)
        {
            Classifications = classifications;
            this.project = project;
            this.dteWithEvents = dteWithEvents;
            this.visualStudioTracer = visualStudioTracer;

            parser = new GherkinTextBufferParser(this, visualStudioTracer);
            analyzer = new GherkinScopeAnalyzer(this, visualStudioTracer);
            GherkinProcessingScheduler = new GherkinProcessingScheduler(visualStudioTracer);
        }

        private void EnsureInitialized()
        {
            if (!initialized)
            {
                lock(this)
                {
                    if (!initialized)
                    {
                        Initialize();
                    }
                }
            }
        }

        private void Initialize()
        {
            specFlowProjectConfiguration = LoadConfiguration();
            gherkinDialectServices = new GherkinDialectServices(specFlowProjectConfiguration.GeneratorConfiguration.FeatureLanguage);

            featureFilesTracker = new ProjectFeatureFilesTracker(this);
            featureFilesTracker.Initialized += FeatureFilesTrackerOnInitialized;

            appConfigTracker = new VsProjectFileTracker(project, "App.config", dteWithEvents, visualStudioTracer);
            appConfigTracker.FileChanged += AppConfigTrackerOnFileChanged;
            appConfigTracker.FileOutOfScope += AppConfigTrackerOnFileOutOfScope;

            projectStepSuggestionProvider = new ProjectStepSuggestionProvider(this);
            initialized = true;

            projectStepSuggestionProvider.Initialize();
            featureFilesTracker.Run();
        }

        private void FeatureFilesTrackerOnInitialized()
        {
            //compare generated file versions with the generator version
            Version generatorVersion = SpecFlowProjectConfiguration.GeneratorConfiguration.GeneratorVersion;
            if (generatorVersion == null)
                return;

            // we reset the last numbers as we don't want to force generating the files for every build
            generatorVersion = new Version(generatorVersion.Major, generatorVersion.Minor, 0, 0);

            Func<FeatureFileInfo, bool> outOfDateFiles = ffi => ffi.GeneratorVersion != null && ffi.GeneratorVersion < generatorVersion;
            if (featureFilesTracker.FeatureFiles.Any(outOfDateFiles))
            {
                var questionResult = MessageBox.Show(
                    "SpecFlow detected that some of the feature files were generated with an earlier version of SpecFlow. Do you want to re-generate them now?",
                    "SpecFlow Generator Version Change",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1);

                if (questionResult != DialogResult.Yes)
                    return;

                featureFilesTracker.ReGenerateAll(outOfDateFiles);
            }
        }

        private void ConfirmReGenerateFilesOnConfigChange()
        {
            var questionResult = MessageBox.Show(
                "SpecFlow detected changes in the configuration that might require re-generating the feature files. Do you want to re-generate them now?", 
                "SpecFlow Configuration Changes",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, 
                MessageBoxDefaultButton.Button1);

            if (questionResult != DialogResult.Yes)
                return;

            featureFilesTracker.ReGenerateAll();
        }

        private void AppConfigTrackerOnFileChanged(ProjectItem appConfigItem)
        {
            var newConfig = LoadConfiguration();
            if (newConfig.Equals(SpecFlowProjectConfiguration)) 
                return;

            bool dialectServicesChanged = !newConfig.GeneratorConfiguration.FeatureLanguage.Equals(GherkinDialectServices.DefaultLanguage);

            specFlowProjectConfiguration = newConfig;
            OnSpecFlowProjectConfigurationChanged();

            if (dialectServicesChanged)
            {
                gherkinDialectServices = new GherkinDialectServices(SpecFlowProjectConfiguration.GeneratorConfiguration.FeatureLanguage);
                OnGherkinDialectServicesChanged();
            }
        }

        private void AppConfigTrackerOnFileOutOfScope(ProjectItem projectItem, string projectRelativeFileName)
        {
            AppConfigTrackerOnFileChanged(projectItem);                
        }

        private SpecFlowProjectConfiguration LoadConfiguration()
        {
            return DteProjectReader.LoadSpecFlowConfigurationFromDteProject(project) ?? new SpecFlowProjectConfiguration();
        }

        private void OnSpecFlowProjectConfigurationChanged()
        {
            this.visualStudioTracer.Trace("SpecFlow configuration changed", "VsProjectScope");
            if (SpecFlowProjectConfigurationChanged != null)
                SpecFlowProjectConfigurationChanged(this, EventArgs.Empty);

            ConfirmReGenerateFilesOnConfigChange();
        }

        private void OnGherkinDialectServicesChanged()
        {
            this.visualStudioTracer.Trace("default language changed", "VsProjectScope");
            if (GherkinDialectServicesChanged != null)
                GherkinDialectServicesChanged(this, EventArgs.Empty);
        }

        public GherkinTextBufferParser GherkinTextBufferParser
        {
            get { return parser; }
        }

        public GherkinScopeAnalyzer GherkinScopeAnalyzer
        {
            get
            {
                return null;
                //TODO: re-enable if analyzer is implemented 
                //return analyzer;
            }
        }

        public void Dispose()
        {
            GherkinProcessingScheduler.Dispose();
            if (appConfigTracker != null)
            {
                appConfigTracker.FileChanged -= AppConfigTrackerOnFileChanged;
                appConfigTracker.FileOutOfScope -= AppConfigTrackerOnFileOutOfScope;
                appConfigTracker.Dispose();
            }
            if (projectStepSuggestionProvider != null)
            {
                projectStepSuggestionProvider.Dispose();
            }
            if (featureFilesTracker != null)
            {
                featureFilesTracker.Initialized -= FeatureFilesTrackerOnInitialized;
                featureFilesTracker.Dispose();
            }
        }
    }
}