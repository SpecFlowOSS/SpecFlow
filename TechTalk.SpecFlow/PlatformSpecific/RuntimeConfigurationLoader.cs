using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
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
        //RuntimeConfiguration LoadAppConfig(RuntimeConfiguration runtimeConfiguration, ConfigurationSectionHandler configSection);
    }

    public class RuntimeConfigurationLoader : IRuntimeConfigurationLoader
    {
        private readonly ITraceListener _traceListener;
        private JsonConfigurationLoader _jsonConfigurationLoader;
        private AppConfigConfigurationLoader _appConfigConfigurationLoader;
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


        public RuntimeConfigurationLoader(ITraceListener traceListener)
        {
            _traceListener = traceListener;
            _jsonConfigurationLoader = new JsonConfigurationLoader();
            _appConfigConfigurationLoader = new AppConfigConfigurationLoader();
            
        }

        public RuntimeConfiguration Load(RuntimeConfiguration runtimeConfiguration)
        {
            if (HasJsonConfig)
            {
                _traceListener.WriteToolOutput("Using specflow.json for configuration");
                return LoadJson(runtimeConfiguration);
            }

            if (HasAppConfig)
            {
                _traceListener.WriteToolOutput("Using app.config for configuration");
                return LoadAppConfig(runtimeConfiguration);
            }

            return GetDefault();
        }

        public static RuntimeConfiguration GetDefault()
        {
            return new RuntimeConfiguration(new ContainerRegistrationCollection(), 
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

            return _appConfigConfigurationLoader.LoadAppConfig(runtimeConfiguration, configSection);
        }

        


        private RuntimeConfiguration LoadJson(RuntimeConfiguration runtimeConfiguration)
        {
            var jsonContent = @"";

            return _jsonConfigurationLoader.LoadJson(runtimeConfiguration, jsonContent);
        }

        

        public bool HasAppConfig => ConfigurationManager.GetSection("specFlow") != null;

        public bool HasJsonConfig => false;

        
    }
}
