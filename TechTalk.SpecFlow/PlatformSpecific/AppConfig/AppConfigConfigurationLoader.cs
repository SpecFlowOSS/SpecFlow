using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using BoDi;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.PlatformSpecific.AppConfig
{
    public class AppConfigConfigurationLoader
    {
        public RuntimeConfiguration LoadAppConfig(RuntimeConfiguration runtimeConfiguration, ConfigurationSectionHandler configSection)
        {
            if (configSection == null) throw new ArgumentNullException(nameof(configSection));

            ContainerRegistrationCollection runtimeContainerRegistrationCollection = runtimeConfiguration.CustomDependencies;
            ContainerRegistrationCollection generatorContainerRegistrationCollection = runtimeConfiguration.GeneratorCustomDependencies;
            CultureInfo featureLanguage = runtimeConfiguration.FeatureLanguage;
            CultureInfo toolLanguage = runtimeConfiguration.ToolLanguage;
            CultureInfo bindingCulture = runtimeConfiguration.BindingCulture;
            string runtimeUnitTestProvider = runtimeConfiguration.UnitTestProvider;
            bool detectAmbiguousMatches = runtimeConfiguration.DetectAmbiguousMatches;
            bool stopAtFirstError = runtimeConfiguration.StopAtFirstError;
            MissingOrPendingStepsOutcome missingOrPendingStepsOutcome = runtimeConfiguration.MissingOrPendingStepsOutcome;
            bool traceSuccessfulSteps = runtimeConfiguration.TraceSuccessfulSteps;
            bool traceTimings = runtimeConfiguration.TraceTimings;
            TimeSpan minTracedDuration = runtimeConfiguration.MinTracedDuration;
            StepDefinitionSkeletonStyle stepDefinitionSkeletonStyle = runtimeConfiguration.StepDefinitionSkeletonStyle;
            List<string> additionalStepAssemblies = runtimeConfiguration.AdditionalStepAssemblies;
            List<PluginDescriptor> pluginDescriptors = runtimeConfiguration.Plugins;

            bool allowRowTests = runtimeConfiguration.AllowRowTests;
            string generatorPath = runtimeConfiguration.GeneratorPath;
            bool allowDebugGeneratedFiles = runtimeConfiguration.AllowDebugGeneratedFiles;


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
                    runtimeContainerRegistrationCollection = configSection.Runtime.Dependencies;
                }
            }

            if (IsSpecified((configSection.Generator)))
            {
                allowDebugGeneratedFiles = configSection.Generator.AllowDebugGeneratedFiles;
                allowRowTests = configSection.Generator.AllowRowTests;
                generatorPath = configSection.Generator.GeneratorPath;

                if (IsSpecified(configSection.Generator.Dependencies))
                {
                    generatorContainerRegistrationCollection = configSection.Generator.Dependencies;
                }
            }

            if (IsSpecified(configSection.UnitTestProvider))
            {
                if (!string.IsNullOrEmpty(configSection.UnitTestProvider.RuntimeProvider))
                {
                    //compatibility mode, we simulate a custom dependency
                    runtimeUnitTestProvider = "custom";
                    runtimeContainerRegistrationCollection.Add(configSection.UnitTestProvider.RuntimeProvider, typeof(IUnitTestRuntimeProvider).AssemblyQualifiedName, runtimeUnitTestProvider);
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
                    runtimeContainerRegistrationCollection.Add(configSection.Trace.Listener, typeof(ITraceListener).AssemblyQualifiedName);
                }

                traceSuccessfulSteps = configSection.Trace.TraceSuccessfulSteps;
                traceTimings = configSection.Trace.TraceTimings;
                minTracedDuration = configSection.Trace.MinTracedDuration;
                stepDefinitionSkeletonStyle = configSection.Trace.StepDefinitionSkeletonStyle;
            }

            foreach (var element in configSection.StepAssemblies)
            {
                var assemblyName = ((StepAssemblyConfigElement)element).Assembly;
                additionalStepAssemblies.Add(assemblyName);
            }

            foreach (PluginConfigElement plugin in configSection.Plugins)
            {
                pluginDescriptors.Add(plugin.ToPluginDescriptor());
            }

            return new RuntimeConfiguration(ConfigSource.AppConfig, 
                                            runtimeContainerRegistrationCollection,
                                            generatorContainerRegistrationCollection,
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
                                            pluginDescriptors,
                                            allowDebugGeneratedFiles,
                                            allowRowTests,
                                            generatorPath
                                            );
        }

        private bool IsSpecified(ConfigurationElement configurationElement)
        {
            return configurationElement != null && configurationElement.ElementInformation.IsPresent;
        }
    }
}
