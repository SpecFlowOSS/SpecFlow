using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using BoDi;
using Newtonsoft.Json;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Compatibility;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.PlatformSpecific
{
    public class RuntimeConfigurationLoader
    {
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

        public RuntimeConfiguration Load(RuntimeConfiguration runtimeConfiguration)
        {
            if (HasJsonConfig)
            {
                return LoadJson(runtimeConfiguration);
            }

            if (HasAppConfig)
            {
                return LoadAppConfig(runtimeConfiguration);
            }

            return GetDefault();
        }

        public static RuntimeConfiguration GetDefault()
        {
            return new RuntimeConfiguration(new ContainerRegistrationCollection(), 
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
                                            DefaultPluginDescriptors);
        }

        private RuntimeConfiguration LoadAppConfig(RuntimeConfiguration runtimeConfiguration)
        {
            var configSection = ConfigurationManager.GetSection("specFlow") as ConfigurationSectionHandler;

            return LoadAppConfig(runtimeConfiguration, configSection);
        }

        public RuntimeConfiguration LoadAppConfig(RuntimeConfiguration runtimeConfiguration, ConfigurationSectionHandler configSection)
        {
            if (configSection == null) throw new ArgumentNullException(nameof(configSection));

            ContainerRegistrationCollection containerRegistrationCollection = runtimeConfiguration.CustomDependencies;
            CultureInfo featureLanguage = runtimeConfiguration.FeatureLanguage;
            CultureInfo toolLanguage = runtimeConfiguration.ToolLanguage;
            CultureInfo bindingCulture = runtimeConfiguration.BindingCulture;
            string runtimeUnitTestProvider = runtimeConfiguration.RuntimeUnitTestProvider;
            bool detectAmbiguousMatches = runtimeConfiguration.DetectAmbiguousMatches;
            bool stopAtFirstError = runtimeConfiguration.StopAtFirstError;
            MissingOrPendingStepsOutcome missingOrPendingStepsOutcome = runtimeConfiguration.MissingOrPendingStepsOutcome;
            bool traceSuccessfulSteps = runtimeConfiguration.TraceSuccessfulSteps;
            bool traceTimings = runtimeConfiguration.TraceTimings;
            TimeSpan minTracedDuration = runtimeConfiguration.MinTracedDuration;
            StepDefinitionSkeletonStyle stepDefinitionSkeletonStyle = runtimeConfiguration.StepDefinitionSkeletonStyle;
            List<string> additionalStepAssemblies = runtimeConfiguration.AdditionalStepAssemblies;
            List<PluginDescriptor> pluginDescriptors = runtimeConfiguration.Plugins;


            if (IsSpecified(configSection.Language))
            {
                featureLanguage = CultureInfo.GetCultureInfo(configSection.Language.Feature);
                toolLanguage = string.IsNullOrEmpty(configSection.Language.Tool) ? CultureInfo.GetCultureInfo(configSection.Language.Feature) : CultureInfo.GetCultureInfo(configSection.Language.Tool);
            }

            if (IsSpecified(configSection.BindingCulture))
            {
                bindingCulture = CultureInfo.GetCultureInfo(configSection.BindingCulture.Name);
            }

            if (IsSpecified(configSection.Runtime))
            {
                detectAmbiguousMatches = configSection.Runtime.DetectAmbiguousMatches;

                stopAtFirstError = configSection.Runtime.StopAtFirstError;
                missingOrPendingStepsOutcome = configSection.Runtime.MissingOrPendingStepsOutcome;

                if (IsSpecified(configSection.Runtime.Dependencies))
                {
                    containerRegistrationCollection = configSection.Runtime.Dependencies;
                }
            }

            if (IsSpecified(configSection.UnitTestProvider))
            {
                if (!string.IsNullOrEmpty(configSection.UnitTestProvider.RuntimeProvider))
                {
                    //compatibility mode, we simulate a custom dependency
                    runtimeUnitTestProvider = "custom";
                    containerRegistrationCollection.Add(configSection.UnitTestProvider.RuntimeProvider, typeof(IUnitTestRuntimeProvider).AssemblyQualifiedName, runtimeUnitTestProvider);
                }
                else
                {
                    runtimeUnitTestProvider = configSection.UnitTestProvider.Name;
                }
            }


            if (IsSpecified(configSection.Trace))
            {
                if (!string.IsNullOrEmpty(configSection.Trace.Listener)) // backwards compatibility
                {
                    containerRegistrationCollection.Add(configSection.Trace.Listener, typeof(ITraceListener).AssemblyQualifiedName);
                }

                traceSuccessfulSteps = configSection.Trace.TraceSuccessfulSteps;
                traceTimings = configSection.Trace.TraceTimings;
                minTracedDuration = configSection.Trace.MinTracedDuration;
                stepDefinitionSkeletonStyle = configSection.Trace.StepDefinitionSkeletonStyle;
            }

            foreach (var element in configSection.StepAssemblies)
            {
                var assemblyName = ((StepAssemblyConfigElement) element).Assembly;
                additionalStepAssemblies.Add(assemblyName);
            }

            foreach (PluginConfigElement plugin in configSection.Plugins)
            {
                pluginDescriptors.Add(plugin.ToPluginDescriptor());
            }

            return new RuntimeConfiguration(containerRegistrationCollection, 
                                            featureLanguage, 
                                            toolLanguage, 
                                            bindingCulture,
                                            runtimeUnitTestProvider, 
                                            detectAmbiguousMatches, 
                                            stopAtFirstError, 
                                            missingOrPendingStepsOutcome,
                                            traceSuccessfulSteps, 
                                            traceTimings, 
                                            minTracedDuration, 
                                            stepDefinitionSkeletonStyle, 
                                            additionalStepAssemblies,
                                            pluginDescriptors);
        }


        private RuntimeConfiguration LoadJson(RuntimeConfiguration runtimeConfiguration)
        {
            var jsonContent = @"";

            return LoadJson(runtimeConfiguration, jsonContent);
        }

        public RuntimeConfiguration LoadJson(RuntimeConfiguration runtimeConfiguration, string jsonContent)
        {
            if (String.IsNullOrWhiteSpace(jsonContent)) throw new ArgumentNullException(nameof(jsonContent));

            var jsonConfig = JsonConvert.DeserializeObject<JsonConfig.JsonConfig>(jsonContent);


            ContainerRegistrationCollection containerRegistrationCollection = runtimeConfiguration.CustomDependencies;
            CultureInfo featureLanguage = runtimeConfiguration.FeatureLanguage;
            CultureInfo toolLanguage = runtimeConfiguration.ToolLanguage;
            CultureInfo bindingCulture = runtimeConfiguration.BindingCulture;
            string runtimeUnitTestProvider = runtimeConfiguration.RuntimeUnitTestProvider;
            bool detectAmbiguousMatches = runtimeConfiguration.DetectAmbiguousMatches;
            bool stopAtFirstError = runtimeConfiguration.StopAtFirstError;
            MissingOrPendingStepsOutcome missingOrPendingStepsOutcome = runtimeConfiguration.MissingOrPendingStepsOutcome;
            bool traceSuccessfulSteps = runtimeConfiguration.TraceSuccessfulSteps;
            bool traceTimings = runtimeConfiguration.TraceTimings;
            TimeSpan minTracedDuration = runtimeConfiguration.MinTracedDuration;
            StepDefinitionSkeletonStyle stepDefinitionSkeletonStyle = runtimeConfiguration.StepDefinitionSkeletonStyle;
            List<string> additionalStepAssemblies = runtimeConfiguration.AdditionalStepAssemblies;
            List<PluginDescriptor> pluginDescriptors = runtimeConfiguration.Plugins;


            var specFlowElement = jsonConfig.SpecFlow;
            if (specFlowElement.Language != null)
            {
                featureLanguage = CultureInfo.GetCultureInfo(specFlowElement.Language.Feature);
                toolLanguage = string.IsNullOrEmpty(specFlowElement.Language.Tool) ? CultureInfo.GetCultureInfo(specFlowElement.Language.Feature) : CultureInfo.GetCultureInfo(specFlowElement.Language.Tool);
            }

            if (specFlowElement.BindingCulture != null)
            {
                bindingCulture = CultureInfo.GetCultureInfo(specFlowElement.BindingCulture.Name);
            }

            if (specFlowElement.UnitTestProvider != null)
            {
                runtimeUnitTestProvider = specFlowElement.UnitTestProvider.Name;
            }

            if (specFlowElement.Runtime != null)
            {
                missingOrPendingStepsOutcome = specFlowElement.Runtime.MissingOrPendingStepsOutcome;
                detectAmbiguousMatches = specFlowElement.Runtime.DetectAmbiguousMatches;
                stopAtFirstError = specFlowElement.Runtime.StopAtFirstError;
            }

            if (specFlowElement.Trace != null)
            {
                traceSuccessfulSteps = specFlowElement.Trace.TraceSuccessfulSteps;
                traceTimings = specFlowElement.Trace.TraceTimings;
                minTracedDuration = specFlowElement.Trace.MinTracedDuration;
                stepDefinitionSkeletonStyle = specFlowElement.Trace.StepDefinitionSkeletonStyle;
            }

            if (specFlowElement.StepAssemblies != null)
            {
                foreach (var stepAssemblyEntry in specFlowElement.StepAssemblies)
                {
                    additionalStepAssemblies.Add(stepAssemblyEntry.Assembly);
                }
            }

            if (specFlowElement.Plugins != null)
            {
                foreach (var pluginEntry in specFlowElement.Plugins)
                {
                    pluginDescriptors.Add(new PluginDescriptor(pluginEntry.Name, pluginEntry.Path, pluginEntry.Type, pluginEntry.Parameters));
                }
            }


            return new RuntimeConfiguration(containerRegistrationCollection,
                                            featureLanguage,
                                            toolLanguage,
                                            bindingCulture,
                                            runtimeUnitTestProvider,
                                            detectAmbiguousMatches,
                                            stopAtFirstError,
                                            missingOrPendingStepsOutcome,
                                            traceSuccessfulSteps,
                                            traceTimings,
                                            minTracedDuration,
                                            stepDefinitionSkeletonStyle,
                                            additionalStepAssemblies,
                                            pluginDescriptors);
        }

        public bool HasAppConfig => ConfigurationManager.GetSection("specFlow") != null;

        public bool HasJsonConfig => false;

        private bool IsSpecified(ConfigurationElement configurationElement)
        {
            return configurationElement != null && configurationElement.ElementInformation.IsPresent;
        }
    }
}
