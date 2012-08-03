using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using EnvDTE;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Vs2010Integration.Generator;
using TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor;
using TechTalk.SpecFlow.Vs2010Integration.StepSuggestions;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public class VsProjectScope : IProjectScope
    {
        private readonly Project project;
        private readonly DteWithEvents dteWithEvents;
        private readonly IVisualStudioTracer tracer;
        private readonly IIntegrationOptionsProvider integrationOptionsProvider;
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
        private IStepDefinitionMatchService stepDefinitionMatchService = null;

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

        public IStepDefinitionMatchService BindingMatchService
        {
            get
            {
                EnsureInitialized();
                return stepDefinitionMatchService;
            }
        }

        public Project Project { get { return project; } }
        public IIdeTracer Tracer { get { return tracer; } }
        internal DteWithEvents DteWithEvents { get { return dteWithEvents; } }

        public IIntegrationOptionsProvider IntegrationOptionsProvider
        {
            get { return integrationOptionsProvider; }
        }

        public event Action SpecFlowProjectConfigurationChanged;
        public event Action GherkinDialectServicesChanged;

        internal VsProjectScope(Project project, DteWithEvents dteWithEvents, GherkinFileEditorClassifications classifications, IVisualStudioTracer tracer, IIntegrationOptionsProvider integrationOptionsProvider)
        {
            Classifications = classifications;
            this.project = project;
            this.dteWithEvents = dteWithEvents;
            this.tracer = tracer;
            this.integrationOptionsProvider = integrationOptionsProvider;

            var integrationOptions = integrationOptionsProvider.GetOptions();

            parser = new GherkinTextBufferParser(this, tracer);
//TODO: enable when analizer is implemented
//            if (integrationOptions.EnableAnalysis)
//                analyzer = new GherkinScopeAnalyzer(this, visualStudioTracer);

            GherkinProcessingScheduler = new GherkinProcessingScheduler(tracer, integrationOptions.EnableAnalysis);

            GeneratorServices = new VsGeneratorServices(project, new VsSpecFlowConfigurationReader(project, tracer), tracer);
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

        private class StepDefinitionMatchServiceWithOnlySimpleTypeConverter : StepDefinitionMatchService
        {
            public StepDefinitionMatchServiceWithOnlySimpleTypeConverter(IBindingRegistry bindingRegistry) : base(bindingRegistry, new OnlySimpleConverter())
            {
            }

            protected override IEnumerable<BindingMatch> GetCandidatingBindingsForBestMatch(StepInstance stepInstance, CultureInfo bindingCulture)
            {
                var normalResult = base.GetCandidatingBindingsForBestMatch(stepInstance, bindingCulture).ToList();
                if (normalResult.Count > 0)
                    return normalResult;

                return GetCandidatingBindings(stepInstance, bindingCulture, useParamMatching: false); // we disable param checking
            }
        }

        private class OnlySimpleConverter : IStepArgumentTypeConverter
        {
            public object Convert(object value, IBindingType typeToConvertTo, CultureInfo cultureInfo)
            {
                throw new NotSupportedException();
            }

            public bool CanConvert(object value, IBindingType typeToConvertTo, CultureInfo cultureInfo)
            {
                if (!(typeToConvertTo is RuntimeBindingType))
                {
                    Type systemType = Type.GetType(typeToConvertTo.FullName, false);
                    if (systemType == null)
                        return false;
                    typeToConvertTo = new RuntimeBindingType(systemType);
                }

                return StepArgumentTypeConverter.CanConvertSimple(typeToConvertTo, value, cultureInfo);
            }
        }

        private void Initialize()
        {
            tracer.Trace("Initializing...", "VsProjectScope");
            try
            {
                specFlowProjectConfiguration = LoadConfiguration();
                gherkinDialectServices = new GherkinDialectServices(specFlowProjectConfiguration.GeneratorConfiguration.FeatureLanguage);

                appConfigTracker = new VsProjectFileTracker(project, "App.config", dteWithEvents, tracer);
                appConfigTracker.FileChanged += AppConfigTrackerOnFileChanged;
                appConfigTracker.FileOutOfScope += AppConfigTrackerOnFileOutOfScope;

                var enableAnalysis = integrationOptionsProvider.GetOptions().EnableAnalysis;
                if (enableAnalysis)
                {
                    featureFilesTracker = new ProjectFeatureFilesTracker(this);
                    featureFilesTracker.Ready += FeatureFilesTrackerOnReady;

                    bindingFilesTracker = new BindingFilesTracker(this);

                    stepSuggestionProvider = new VsStepSuggestionProvider(this);
                    stepSuggestionProvider.Ready += StepSuggestionProviderOnReady;
                    stepDefinitionMatchService = new StepDefinitionMatchServiceWithOnlySimpleTypeConverter(stepSuggestionProvider);
                }
                tracer.Trace("Initialized", "VsProjectScope");
                initialized = true;

                if (enableAnalysis)
                {
                    tracer.Trace("Starting analysis services...", "VsProjectScope");

                    stepSuggestionProvider.Initialize();
                    bindingFilesTracker.Initialize();
                    featureFilesTracker.Initialize();

                    LoadStepMap();

                    bindingFilesTracker.Run();
                    featureFilesTracker.Run();

                    dteWithEvents.BuildEvents.OnBuildDone += BuildEventsOnOnBuildDone;

                    tracer.Trace("Analysis services started", "VsProjectScope");
                }
                else
                {
                    tracer.Trace("Analysis services disabled", "VsProjectScope");
                }
            }
            catch(Exception exception)
            {
                tracer.Trace("Exception: " + exception, "VsProjectScope");
            }
        }

        private void FeatureFilesTrackerOnReady()
        {
            //compare generated file versions with the generator version
            Version generatorVersion = GeneratorServices.GetGeneratorVersion(); //TODO: cache GeneratorVersion
            if (generatorVersion == null)
                return;

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
            ISpecFlowConfigurationReader configurationReader = new VsSpecFlowConfigurationReader(project, tracer); //TODO: load through DI
            IGeneratorConfigurationProvider configurationLoader = new GeneratorConfigurationProvider(); //TODO: load through DI

            try
            {
                return configurationLoader.LoadConfiguration(configurationReader.ReadConfiguration());
            }
            catch(Exception exception)
            {
                tracer.Trace("Configuration loading error: " + exception, "VsProjectScope");
                return new SpecFlowProjectConfiguration();
            }
        }

        private void OnSpecFlowProjectConfigurationChanged()
        {
            this.tracer.Trace("SpecFlow configuration changed", "VsProjectScope");
            if (SpecFlowProjectConfigurationChanged != null)
                SpecFlowProjectConfigurationChanged();

            GeneratorServices.InvalidateSettings();

            ConfirmReGenerateFilesOnConfigChange();
        }

        private void OnGherkinDialectServicesChanged()
        {
            this.tracer.Trace("default language changed", "VsProjectScope");
            if (GherkinDialectServicesChanged != null)
                GherkinDialectServicesChanged();
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

        private void StepSuggestionProviderOnReady()
        {
            SaveStepMap();
        }

        public void Dispose()
        {
            dteWithEvents.BuildEvents.OnBuildDone -= BuildEventsOnOnBuildDone;
            SaveStepMap();

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
                bindingFilesTracker.Dispose();
            }
        }

        private string stepMapFileName;

        private string GetStepMapFileName()
        {
            return stepMapFileName ?? (stepMapFileName = Path.Combine(Path.GetTempPath(), 
                string.Format(@"specflow-stepmap-{1}-{2}-{0}{3}.cache", VsxHelper.GetProjectUniqueId(project), project.Name, Math.Abs(VsxHelper.GetProjectFolder(project).GetHashCode()), GetConfigurationText())));
        }

        private string GetConfigurationText()
        {
            //TODO: once we can better track config changes, we can also have different cache for the different configs
#if USE_CONFIG_DEPENDENT_CACHE
            try
            {
                return "-" + project.ConfigurationManager.ActiveConfiguration.ConfigurationName + "-" +
                       project.ConfigurationManager.ActiveConfiguration.PlatformName;
            }
            catch(Exception ex)
            {
                tracer.Trace("Unable to get configuration name: " + ex, GetType().Name);
                return "-na";
            }
#else
            return "";
#endif
        }

        private void BuildEventsOnOnBuildDone(vsBuildScope scope, vsBuildAction action)
        {
            SaveStepMap();
        }

        private void SaveStepMap()
        {
            if (featureFilesTracker == null || !featureFilesTracker.IsInitialized ||
                bindingFilesTracker == null || !bindingFilesTracker.IsInitialized)
                return;

            if (!featureFilesTracker.IsStepMapDirty && !bindingFilesTracker.IsStepMapDirty)
            {
                tracer.Trace("Step map up-to-date", typeof(StepMap).Name);
                return;
            }

            var stepMap = StepMap.CreateStepMap(GherkinDialectServices.DefaultLanguage);
            featureFilesTracker.SaveToStepMap(stepMap);
            bindingFilesTracker.SaveToStepMap(stepMap);

            stepMap.SaveToFile(GetStepMapFileName(), tracer);
        }

        private void LoadStepMap()
        {
            var fileName = GetStepMapFileName();
            if (!File.Exists(fileName))
                return;

            var stepMap = StepMap.LoadFromFile(fileName, tracer);
            if (stepMap != null)
            {
                if (stepMap.DefaultLanguage.Equals(GherkinDialectServices.DefaultLanguage)) // if default language changed in config => ignore cache
                    featureFilesTracker.LoadFromStepMap(stepMap);
                bindingFilesTracker.LoadFromStepMap(stepMap);
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
            if (project.FullName.EndsWith(".fsproj"))
                return ProgrammingLanguage.FSharp;
            return ProgrammingLanguage.Other;
        }
    }
}