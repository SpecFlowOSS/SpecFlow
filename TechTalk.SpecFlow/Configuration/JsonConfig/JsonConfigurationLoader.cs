using System;
using System.Collections.Generic;
using System.Globalization;
using BoDi;
using TechTalk.SpecFlow.BindingSkeletons;
using Utf8Json;

namespace TechTalk.SpecFlow.Configuration.JsonConfig
{
    public class JsonConfigurationLoader
    {
        public SpecFlowConfiguration LoadJson(SpecFlowConfiguration specFlowConfiguration, string jsonContent)
        {
            if (String.IsNullOrWhiteSpace(jsonContent)) throw new ArgumentNullException(nameof(jsonContent));

            var jsonConfig = JsonSerializer.Deserialize<JsonConfig>(jsonContent);

            ContainerRegistrationCollection containerRegistrationCollection = specFlowConfiguration.CustomDependencies;
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
            bool allowRowTests = specFlowConfiguration.AllowRowTests;
            bool allowDebugGeneratedFiles = specFlowConfiguration.AllowDebugGeneratedFiles;
            bool markFeaturesParallelizable = specFlowConfiguration.MarkFeaturesParallelizable;
            string[] skipParallelizableMarkerForTags = specFlowConfiguration.SkipParallelizableMarkerForTags;
            ObsoleteBehavior obsoleteBehavior = specFlowConfiguration.ObsoleteBehavior;
            CucumberMessagesConfiguration cucumberMessagesConfiguration = specFlowConfiguration.CucumberMessagesConfiguration;

            if (jsonConfig.Language != null)
            {
                if (jsonConfig.Language.Feature.IsNotNullOrWhiteSpace())
                {
                    featureLanguage = CultureInfo.GetCultureInfo(jsonConfig.Language.Feature);
                }
            }

            if (jsonConfig.BindingCulture != null)
            {
                if (jsonConfig.BindingCulture.Name.IsNotNullOrWhiteSpace())
                {
                    bindingCulture = CultureInfo.GetCultureInfo(jsonConfig.BindingCulture.Name);
                }
            }

            if (jsonConfig.Runtime != null)
            {
                missingOrPendingStepsOutcome = jsonConfig.Runtime.MissingOrPendingStepsOutcome;
                stopAtFirstError = jsonConfig.Runtime.StopAtFirstError;
                obsoleteBehavior = jsonConfig.Runtime.ObsoleteBehavior;

                if (jsonConfig.Runtime.Dependencies != null)
                {
                    foreach (var runtimeDependency in jsonConfig.Runtime.Dependencies)
                    {
                        containerRegistrationCollection.Add(runtimeDependency.ImplementationType, runtimeDependency.InterfaceType);
                    }
                }
            }

            if (jsonConfig.Generator != null)
            {
                allowDebugGeneratedFiles = jsonConfig.Generator.AllowDebugGeneratedFiles;
                allowRowTests = jsonConfig.Generator.AllowRowTests;
                markFeaturesParallelizable = jsonConfig.Generator.MarkFeaturesParallelizable;
                skipParallelizableMarkerForTags = jsonConfig.Generator.SkipParallelizableMarkerForTags?.ToArray();
            }

            if (jsonConfig.Trace != null)
            {
                traceSuccessfulSteps = jsonConfig.Trace.TraceSuccessfulSteps;
                traceTimings = jsonConfig.Trace.TraceTimings;
                minTracedDuration = jsonConfig.Trace.MinTracedDuration;
                stepDefinitionSkeletonStyle = jsonConfig.Trace.StepDefinitionSkeletonStyle;
            }

            if (jsonConfig.StepAssemblies != null)
            {
                foreach (var stepAssemblyEntry in jsonConfig.StepAssemblies)
                {
                    additionalStepAssemblies.Add(stepAssemblyEntry.Assembly);
                }
            }

            if (jsonConfig.CucumberMessages != null)
            {
                cucumberMessagesConfiguration.Enabled = jsonConfig.CucumberMessages.Enabled;

                if (jsonConfig.CucumberMessages.Sinks != null)
                {
                    foreach (var cucumberMessageSinkElement in jsonConfig.CucumberMessages.Sinks)
                    {
                        cucumberMessagesConfiguration.Sinks.Add(new CucumberMessagesSink(cucumberMessageSinkElement.Type, cucumberMessageSinkElement.Path));
                    }
                }
            }

            return new SpecFlowConfiguration(ConfigSource.Json,
                                            containerRegistrationCollection,
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
                                            markFeaturesParallelizable,
                                            skipParallelizableMarkerForTags,
                                            obsoleteBehavior,
                                            cucumberMessagesConfiguration
                                            );
        }
    }
}
