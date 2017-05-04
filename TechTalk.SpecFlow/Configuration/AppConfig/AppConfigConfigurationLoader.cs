using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using BoDi;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.Configuration.AppConfig
{
    public class AppConfigConfigurationLoader
    {
        public Configuration.SpecFlowConfiguration LoadAppConfig(Configuration.SpecFlowConfiguration specFlowConfiguration, ConfigurationSectionHandler configSection)
        {
            if (configSection == null) throw new ArgumentNullException(nameof(configSection));

            ContainerRegistrationCollection runtimeContainerRegistrationCollection = specFlowConfiguration.CustomDependencies;
            ContainerRegistrationCollection generatorContainerRegistrationCollection = specFlowConfiguration.GeneratorCustomDependencies;
            CultureInfo featureLanguage = specFlowConfiguration.FeatureLanguage;
            CultureInfo bindingCulture = specFlowConfiguration.BindingCulture;
            string runtimeUnitTestProvider = specFlowConfiguration.UnitTestProvider;
            bool stopAtFirstError = specFlowConfiguration.StopAtFirstError;
            MissingOrPendingStepsOutcome missingOrPendingStepsOutcome = specFlowConfiguration.MissingOrPendingStepsOutcome;
            bool traceSuccessfulSteps = specFlowConfiguration.TraceSuccessfulSteps;
            bool traceTimings = specFlowConfiguration.TraceTimings;
            TimeSpan minTracedDuration = specFlowConfiguration.MinTracedDuration;
            StepDefinitionSkeletonStyle stepDefinitionSkeletonStyle = specFlowConfiguration.StepDefinitionSkeletonStyle;
            List<string> additionalStepAssemblies = specFlowConfiguration.AdditionalStepAssemblies;
            List<PluginDescriptor> pluginDescriptors = specFlowConfiguration.Plugins;

            bool allowRowTests = specFlowConfiguration.AllowRowTests;
            bool allowDebugGeneratedFiles = specFlowConfiguration.AllowDebugGeneratedFiles;

            bool markFeaturesParallelizable = specFlowConfiguration.MarkFeaturesParallelizable;
            string[] skipParallelizableMarkerForTags = specFlowConfiguration.SkipParallelizableMarkerForTags;


            if (IsSpecified(configSection.Language))
            {
                featureLanguage = CultureInfo.GetCultureInfo(configSection.Language.Feature);
            }

            if (IsSpecified(configSection.BindingCulture))
            {
                bindingCulture = CultureInfo.GetCultureInfo(configSection.BindingCulture.Name);
            }

            if (IsSpecified(configSection.Runtime))
            {
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
                markFeaturesParallelizable = configSection.Generator.MarkFeaturesParallelizable;

                if (IsSpecified(configSection.Generator.SkipParallelizableMarkerForTags))
                {
                    skipParallelizableMarkerForTags = configSection.Generator.SkipParallelizableMarkerForTags.Select(i => i.Value).ToArray();
                }

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

            return new SpecFlowConfiguration(ConfigSource.AppConfig, 
                                            runtimeContainerRegistrationCollection,
                                            generatorContainerRegistrationCollection,
                                            featureLanguage,
                                            bindingCulture,
                                            runtimeUnitTestProvider,
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
                                            markFeaturesParallelizable,
                                            skipParallelizableMarkerForTags
                                            );
        }

        private bool IsSpecified(ConfigurationElement configurationElement)
        {
            return configurationElement != null && configurationElement.ElementInformation.IsPresent;
        }
    }
}
