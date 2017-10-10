using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using BoDi;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Compatibility;
using TechTalk.SpecFlow.Configuration.AppConfig;
using TechTalk.SpecFlow.Configuration.JsonConfig;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Configuration
{
    public interface IConfigurationLoader
    {
        SpecFlowConfiguration Load(SpecFlowConfiguration specFlowConfiguration, ISpecFlowConfigurationHolder specFlowConfigurationHolder);

        SpecFlowConfiguration Load(SpecFlowConfiguration specFlowConfiguration);

        SpecFlowConfiguration Update(SpecFlowConfiguration specFlowConfiguration, ConfigurationSectionHandler specFlowConfigSection);

        void TraceConfigSource(ITraceListener traceListener, SpecFlowConfiguration specFlowConfiguration);
    }

    public class ConfigurationLoader : IConfigurationLoader
    {
        private readonly AppConfigConfigurationLoader _appConfigConfigurationLoader;
        //private readonly ObjectContainer _objectContainer;
        private readonly JsonConfigurationLoader _jsonConfigurationLoader;


        public ConfigurationLoader()
        {
            _jsonConfigurationLoader = new JsonConfigurationLoader();
            _appConfigConfigurationLoader = new AppConfigConfigurationLoader();
        }

        private static CultureInfo DefaultFeatureLanguage => CultureInfo.GetCultureInfo(ConfigDefaults.FeatureLanguage);

        private static CultureInfo DefaultToolLanguage => CultureInfoHelper.GetCultureInfo(ConfigDefaults.FeatureLanguage);

        private static CultureInfo DefaultBindingCulture => null;
        private static string DefaultUnitTestProvider => ConfigDefaults.UnitTestProviderName;
        private static bool DefaultDetectAmbiguousMatches => ConfigDefaults.DetectAmbiguousMatches;
        private static bool DefaultStopAtFirstError => ConfigDefaults.StopAtFirstError;

        private static MissingOrPendingStepsOutcome DefaultMissingOrPendingStepsOutcome => ConfigDefaults.MissingOrPendingStepsOutcome;

        private static bool DefaultTraceSuccessfulSteps => ConfigDefaults.TraceSuccessfulSteps;
        private static bool DefaultTraceTimings => ConfigDefaults.TraceTimings;
        private static TimeSpan DefaultMinTracedDuration => TimeSpan.Parse(ConfigDefaults.MinTracedDuration);

        private static StepDefinitionSkeletonStyle DefaultStepDefinitionSkeletonStyle => ConfigDefaults.StepDefinitionSkeletonStyle;

        private static List<string> DefaultAdditionalStepAssemblies => new List<string>();
        private static List<PluginDescriptor> DefaultPluginDescriptors => new List<PluginDescriptor>();
        private static bool DefaultAllowDebugGeneratedFiles => ConfigDefaults.AllowDebugGeneratedFiles;
        private static bool DefaultAllowRowTests => ConfigDefaults.AllowRowTests;
        public static string DefaultGeneratorPath => ConfigDefaults.GeneratorPath;

        public static bool DefaultMarkFeaturesParallelizable => ConfigDefaults.MarkFeaturesParallelizable;
        public static string[] DefaultSkipParallelizableMarkerForTags => ConfigDefaults.SkipParallelizableMarkerForTags;

        public bool HasAppConfig => ConfigurationManager.GetSection("specFlow") != null;

        public bool HasJsonConfig
        {
            get
            {
                var specflowJsonFile = GetSpecflowJsonFilePath();


                return File.Exists(specflowJsonFile);
            }
        }

        public SpecFlowConfiguration Load(SpecFlowConfiguration specFlowConfiguration, ISpecFlowConfigurationHolder specFlowConfigurationHolder)
        {
            if (!specFlowConfigurationHolder.HasConfiguration)
                return GetDefault();

            switch (specFlowConfigurationHolder.ConfigSource)
            {
                case ConfigSource.Default:
                    return GetDefault();
                case ConfigSource.AppConfig:
                    return LoadAppConfig(specFlowConfiguration,
                        ConfigurationSectionHandler.CreateFromXml(specFlowConfigurationHolder.Content));
                case ConfigSource.Json:
                    return LoadJson(specFlowConfiguration, specFlowConfigurationHolder.Content);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public SpecFlowConfiguration Load(SpecFlowConfiguration specFlowConfiguration)
        {
            if (HasJsonConfig)
                return LoadJson(specFlowConfiguration);

            if (HasAppConfig)
                return LoadAppConfig(specFlowConfiguration);

            return GetDefault();
        }

        public SpecFlowConfiguration Update(SpecFlowConfiguration specFlowConfiguration, ConfigurationSectionHandler specFlowConfigSection)
        {
            return LoadAppConfig(specFlowConfiguration, specFlowConfigSection);
        }

        public void TraceConfigSource(ITraceListener traceListener, SpecFlowConfiguration specFlowConfiguration)
        {
            switch (specFlowConfiguration.ConfigSource)
            {
                case ConfigSource.Default:
                    traceListener.WriteToolOutput("Using default config");
                    break;
                case ConfigSource.AppConfig:
                    traceListener.WriteToolOutput("Using app.config");
                    break;
                case ConfigSource.Json:
                    traceListener.WriteToolOutput("Using specflow.json");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public static SpecFlowConfiguration GetDefault()
        {
            return new SpecFlowConfiguration(ConfigSource.Default,
                new ContainerRegistrationCollection(), 
                new ContainerRegistrationCollection(), 
                DefaultFeatureLanguage, 
                DefaultBindingCulture, 
                DefaultUnitTestProvider, 
                DefaultStopAtFirstError, 
                DefaultMissingOrPendingStepsOutcome,
                DefaultTraceSuccessfulSteps, 
                DefaultTraceTimings, 
                DefaultMinTracedDuration,
                DefaultStepDefinitionSkeletonStyle, 
                DefaultAdditionalStepAssemblies, 
                DefaultPluginDescriptors,
                DefaultAllowDebugGeneratedFiles, 
                DefaultAllowRowTests,
                DefaultMarkFeaturesParallelizable,
                DefaultSkipParallelizableMarkerForTags
                );
        }


        private SpecFlowConfiguration LoadAppConfig(SpecFlowConfiguration specFlowConfiguration)
        {
            var configSection = ConfigurationManager.GetSection("specFlow") as ConfigurationSectionHandler;

            return LoadAppConfig(specFlowConfiguration, configSection);
        }

        private SpecFlowConfiguration LoadAppConfig(SpecFlowConfiguration specFlowConfiguration,
            ConfigurationSectionHandler specFlowConfigSection)
        {
            return _appConfigConfigurationLoader.LoadAppConfig(specFlowConfiguration, specFlowConfigSection);
        }


        private SpecFlowConfiguration LoadJson(SpecFlowConfiguration specFlowConfiguration)
        {
            var jsonContent = File.ReadAllText(GetSpecflowJsonFilePath());

            return LoadJson(specFlowConfiguration, jsonContent);
        }

        private SpecFlowConfiguration LoadJson(SpecFlowConfiguration specFlowConfiguration, string jsonContent)
        {
            return _jsonConfigurationLoader.LoadJson(specFlowConfiguration, jsonContent);
        }

        private static string GetSpecflowJsonFilePath()
        {
            var directory = Path.GetDirectoryName(typeof(ConfigurationLoader).Assembly.Location);
            var specflowJsonFile = Path.Combine(directory, "specflow.json");
            return specflowJsonFile;
        }
    }
}