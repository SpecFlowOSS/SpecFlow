using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using BoDi;
using Newtonsoft.Json;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Compatibility;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.PlatformSpecific.AppConfig;
using TechTalk.SpecFlow.PlatformSpecific.JsonConfig;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.PlatformSpecific
{
    

    public interface IRuntimeConfigurationLoader
    {
        RuntimeConfiguration Load(RuntimeConfiguration runtimeConfiguration);
        RuntimeConfiguration Update(RuntimeConfiguration runtimeConfiguration, ConfigurationSectionHandler specFlowConfigSection);
        void PrintConfigSource(ITraceListener traceListener, RuntimeConfiguration runtimeConfiguration);
    }

    public class RuntimeConfigurationLoader : IRuntimeConfigurationLoader
    {
        //private readonly ObjectContainer _objectContainer;
        private readonly JsonConfigurationLoader _jsonConfigurationLoader;
        private readonly AppConfigConfigurationLoader _appConfigConfigurationLoader;
        private static CultureInfo DefaultFeatureLanguage => CultureInfo.GetCultureInfo(ConfigDefaults.FeatureLanguage);
        private static CultureInfo DefaultToolLanguage => CultureInfoHelper.GetCultureInfo(ConfigDefaults.FeatureLanguage);
        private static CultureInfo DefaultBindingCulture => null;
        private static string DefaultRuntimeUnitTestProvider => ConfigDefaults.UnitTestProviderName;
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


        public RuntimeConfigurationLoader()
        {
            _jsonConfigurationLoader = new JsonConfigurationLoader();
            _appConfigConfigurationLoader = new AppConfigConfigurationLoader();
            
        }

        public RuntimeConfiguration Load(RuntimeConfiguration runtimeConfiguration)
        {
            if (HasJsonConfig)
            {

                var configuration = LoadJson(runtimeConfiguration);
                return configuration;
            }

            if (HasAppConfig)
            {
                var configuration = LoadAppConfig(runtimeConfiguration);       
                return configuration;
            }

            return GetDefault();
        }

        public RuntimeConfiguration Update(RuntimeConfiguration runtimeConfiguration, ConfigurationSectionHandler specFlowConfigSection)
        {
            return LoadAppConfig(runtimeConfiguration, specFlowConfigSection);
        }


        public static RuntimeConfiguration GetDefault()
        {
            return new RuntimeConfiguration(ConfigSource.Default,
                                            new ContainerRegistrationCollection(), 
                                            new ContainerRegistrationCollection(), 
                                            DefaultFeatureLanguage,
                                            DefaultToolLanguage, 
                                            DefaultBindingCulture, 
                                            DefaultRuntimeUnitTestProvider,
                                            DefaultDetectAmbiguousMatches, 
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
                                            DefaultGeneratorPath);
        }



        private RuntimeConfiguration LoadAppConfig(RuntimeConfiguration runtimeConfiguration)
        {
            var configSection = ConfigurationManager.GetSection("specFlow") as ConfigurationSectionHandler;

            return LoadAppConfig(runtimeConfiguration, configSection);
        }

        private RuntimeConfiguration LoadAppConfig(RuntimeConfiguration runtimeConfiguration, ConfigurationSectionHandler specFlowConfigSection)
        {
            return _appConfigConfigurationLoader.LoadAppConfig(runtimeConfiguration, specFlowConfigSection);
        }


        private RuntimeConfiguration LoadJson(RuntimeConfiguration runtimeConfiguration)
        {
            var jsonContent = File.ReadAllText(GetSpecflowJsonFilePath());

            return _jsonConfigurationLoader.LoadJson(runtimeConfiguration, jsonContent);
        }

        

        public bool HasAppConfig => ConfigurationManager.GetSection("specFlow") != null;

        public bool HasJsonConfig
        {
            get
            {
                var specflowJsonFile = GetSpecflowJsonFilePath();


                return File.Exists(specflowJsonFile);
            }
        }

        private static string GetSpecflowJsonFilePath()
        {
            var directory = Path.GetDirectoryName(typeof(RuntimeConfigurationLoader).Assembly.Location);
            var specflowJsonFile = Path.Combine(directory, "specflow.json");
            return specflowJsonFile;
        }

        public void PrintConfigSource(ITraceListener traceListener, RuntimeConfiguration runtimeConfiguration)
        {
            switch (runtimeConfiguration.ConfigSource)
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
    }
}
