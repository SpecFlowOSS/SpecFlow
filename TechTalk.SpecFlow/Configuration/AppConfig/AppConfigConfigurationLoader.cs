using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using BoDi;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Tracing;

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
            bool stopAtFirstError = specFlowConfiguration.StopAtFirstError;
            MissingOrPendingStepsOutcome missingOrPendingStepsOutcome = specFlowConfiguration.MissingOrPendingStepsOutcome;
            bool traceSuccessfulSteps = specFlowConfiguration.TraceSuccessfulSteps;
            bool traceTimings = specFlowConfiguration.TraceTimings;
            TimeSpan minTracedDuration = specFlowConfiguration.MinTracedDuration;
            StepDefinitionSkeletonStyle stepDefinitionSkeletonStyle = specFlowConfiguration.StepDefinitionSkeletonStyle;
            List<string> additionalStepAssemblies = specFlowConfiguration.AdditionalStepAssemblies;
            ObsoleteBehavior obsoleteBehavior = specFlowConfiguration.ObsoleteBehavior;
            bool coloredOutput = specFlowConfiguration.ColoredOutput;

            bool allowRowTests = specFlowConfiguration.AllowRowTests;
            bool allowDebugGeneratedFiles = specFlowConfiguration.AllowDebugGeneratedFiles;
            string[] addNonParallelizableMarkerForTags = specFlowConfiguration.AddNonParallelizableMarkerForTags;


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
                obsoleteBehavior = configSection.Runtime.ObsoleteBehavior;

                if (IsSpecified(configSection.Runtime.Dependencies))
                {
                    runtimeContainerRegistrationCollection = configSection.Runtime.Dependencies;
                }
            }

            if (IsSpecified((configSection.Generator)))
            {
                allowDebugGeneratedFiles = configSection.Generator.AllowDebugGeneratedFiles;
                allowRowTests = configSection.Generator.AllowRowTests;

                if (IsSpecified(configSection.Generator.AddNonParallelizableMarkerForTags))
                {
                    addNonParallelizableMarkerForTags = configSection.Generator.AddNonParallelizableMarkerForTags.Select(i => i.Value).ToArray();
                }

                if (IsSpecified(configSection.Generator.Dependencies))
                {
                    generatorContainerRegistrationCollection = configSection.Generator.Dependencies;
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
                coloredOutput = configSection.Trace.ColoredOutput;
            }

            foreach (var element in configSection.StepAssemblies)
            {
                var assemblyName = ((StepAssemblyConfigElement)element).Assembly;
                additionalStepAssemblies.Add(assemblyName);
            }

            return new SpecFlowConfiguration(ConfigSource.AppConfig,
                                            runtimeContainerRegistrationCollection,
                                            generatorContainerRegistrationCollection,
                                            featureLanguage,
                                            bindingCulture,
                                            stopAtFirstError,
                                            missingOrPendingStepsOutcome,
                                            traceSuccessfulSteps,
                                            traceTimings,
                                            minTracedDuration,
                                            stepDefinitionSkeletonStyle,
                                            additionalStepAssemblies,
                                            allowDebugGeneratedFiles,
                                            allowRowTests,
                                            addNonParallelizableMarkerForTags,
                                            obsoleteBehavior,
                                            coloredOutput
                                            );
        }

        private bool IsSpecified(ConfigurationElement configurationElement)
        {
            return configurationElement != null && configurationElement.ElementInformation.IsPresent;
        }
    }
}
