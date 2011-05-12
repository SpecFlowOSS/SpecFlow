using System;
using System.Linq;
using System.Windows.Forms;
using EnvDTE;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Vs2010Integration.Generator;
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
        private readonly IIntegrationOptionsProvider integrationOptionsProvider;
        private readonly IBindingSkeletonProviderFactory bindingSkeletonProviderFactory;
        private readonly GherkinTextBufferParser parser;
        private readonly GherkinScopeAnalyzer analyzer = null;
        public GherkinFileEditorClassifications Classifications { get; private set; }
        public GherkinProcessingScheduler GherkinProcessingScheduler { get; private set; }
        public IGeneratorServices GeneratorServices { get; private set; }

        private bool initialized = false;
        
        // delay initialized members
        private SpecFlowProjectConfiguration specFlowProjectConfiguration = null;
        private GherkinDialectServices gherkinDialectServices = null;
        private VsProjectFileTracker appConfigTracker = null;
        private ProjectFeatureFilesTracker featureFilesTracker = null;
        private BindingFilesTracker bindingFilesTracker = null;
        private VsStepSuggestionProvider stepSuggestionProvider = null;
        private IBindingMatchService bindingMatchService = null;

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

        internal BindingFilesTracker BindingFilesTracker
        {
            get
            {
                EnsureInitialized();
                return bindingFilesTracker;
            }
        }

        public VsStepSuggestionProvider StepSuggestionProvider
        {
            get
            {
                EnsureInitialized();
                return stepSuggestionProvider;
            }
        }

        public IBindingMatchService BindingMatchService
        {
            get
            {
                EnsureInitialized();
                return bindingMatchService;
            }
        }

        public Project Project { get { return project; } }
        public IVisualStudioTracer VisualStudioTracer { get { return visualStudioTracer; } }
        internal DteWithEvents DteWithEvents { get { return dteWithEvents; } }

        public IStepDefinitionSkeletonProvider StepDefinitionSkeletonProvider
        {
            get { return bindingSkeletonProviderFactory.GetProvider(GetTargetLanguage(project), GherkinDialectServices.GetDefaultDialect()); }
        }

        public IIntegrationOptionsProvider IntegrationOptionsProvider
        {
            get { return integrationOptionsProvider; }
        }

        public event EventHandler SpecFlowProjectConfigurationChanged;
        public event EventHandler GherkinDialectServicesChanged;

        internal VsProjectScope(Project project, DteWithEvents dteWithEvents, GherkinFileEditorClassifications classifications, IVisualStudioTracer visualStudioTracer, IIntegrationOptionsProvider integrationOptionsProvider, IBindingSkeletonProviderFactory bindingSkeletonProviderFactory)
        {
            Classifications = classifications;
            this.project = project;
            this.dteWithEvents = dteWithEvents;
            this.visualStudioTracer = visualStudioTracer;
            this.integrationOptionsProvider = integrationOptionsProvider;
            this.bindingSkeletonProviderFactory = bindingSkeletonProviderFactory;

            var integrationOptions = integrationOptionsProvider.GetOptions();

            parser = new GherkinTextBufferParser(this, visualStudioTracer);
//TODO: enable when analizer is implemented
//            if (integrationOptions.EnableAnalysis)
//                analyzer = new GherkinScopeAnalyzer(this, visualStudioTracer);

            GherkinProcessingScheduler = new GherkinProcessingScheduler(visualStudioTracer, integrationOptions.EnableAnalysis);

            GeneratorServices = new VsGeneratorServices(project, new VsSpecFlowConfigurationReader(project, visualStudioTracer), visualStudioTracer);
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

            appConfigTracker = new VsProjectFileTracker(project, "App.config", dteWithEvents, visualStudioTracer);
            appConfigTracker.FileChanged += AppConfigTrackerOnFileChanged;
            appConfigTracker.FileOutOfScope += AppConfigTrackerOnFileOutOfScope;

            var enableAnalysis = integrationOptionsProvider.GetOptions().EnableAnalysis;
            if (enableAnalysis)
            {
                featureFilesTracker = new ProjectFeatureFilesTracker(this);
                featureFilesTracker.Ready += FeatureFilesTrackerOnReady;

                bindingFilesTracker = new BindingFilesTracker(this);

                stepSuggestionProvider = new VsStepSuggestionProvider(this);
                bindingMatchService = new BindingMatchService(stepSuggestionProvider);
            }
            initialized = true;

            if (enableAnalysis)
            {
                stepSuggestionProvider.Initialize();
                bindingFilesTracker.Initialize();
                featureFilesTracker.Initialize();
                bindingFilesTracker.Run();
                featureFilesTracker.Run();
            }
        }

        private void FeatureFilesTrackerOnReady()
        {
            //compare generated file versions with the generator version
            Version generatorVersion = GeneratorServices.GetGeneratorVersion(); //TODO: cache GeneratorVersion
            if (generatorVersion == null)
                return;

            // we reset the last numbers as we don't want to force generating the files for every build
            generatorVersion = new Version(generatorVersion.Major, generatorVersion.Minor, 0, 0); 
                //TODO: consider removing this after the generator versioning has been well established

            Func<FeatureFileInfo, bool> outOfDateFiles = ffi => ffi.GeneratorVersion != null && ffi.GeneratorVersion < generatorVersion;
            if (featureFilesTracker.Files.Any(outOfDateFiles))
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
            ISpecFlowConfigurationReader configurationReader = new VsSpecFlowConfigurationReader(project, visualStudioTracer); //TODO: load through DI
            ISpecFlowProjectConfigurationLoader configurationLoader = new SpecFlowProjectConfigurationLoaderWithoutPlugins(); //TODO: load through DI

            try
            {
                return configurationLoader.LoadConfiguration(configurationReader.ReadConfiguration());
            }
            catch(Exception exception)
            {
                visualStudioTracer.Trace("Configuration loading error: " + exception, "VsProjectScope");
                return new SpecFlowProjectConfiguration();
            }
        }

        private void OnSpecFlowProjectConfigurationChanged()
        {
            this.visualStudioTracer.Trace("SpecFlow configuration changed", "VsProjectScope");
            if (SpecFlowProjectConfigurationChanged != null)
                SpecFlowProjectConfigurationChanged(this, EventArgs.Empty);

            GeneratorServices.InvalidateSettings();

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
                return analyzer;
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
            if (stepSuggestionProvider != null)
            {
                stepSuggestionProvider.Dispose();
            }
            if (featureFilesTracker != null)
            {
                featureFilesTracker.Ready -= FeatureFilesTrackerOnReady;
                featureFilesTracker.Dispose();
            }
            if (bindingFilesTracker != null)
            {
//                bindingFilesTracker.Initialized -= FeatureFilesTrackerOnInitialized;
                bindingFilesTracker.Dispose();
            }
        }

        public static bool IsProjectSupported(Project project)
        {
            return GetTargetLanguage(project) != ProgrammingLanguage.Other;
        }

        public static ProgrammingLanguage GetTargetLanguage(Project project)
        {
            if (project.FullName.EndsWith(".csproj"))
                return ProgrammingLanguage.CSharp;
            if (project.FullName.EndsWith(".vbproj"))
                return ProgrammingLanguage.VB;
            return ProgrammingLanguage.Other;
        }
    }
}