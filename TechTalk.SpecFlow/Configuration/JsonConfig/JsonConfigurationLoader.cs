using System;
using System.Collections.Generic;
using System.Globalization;
using BoDi;
using Newtonsoft.Json;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Plugins;

namespace TechTalk.SpecFlow.Configuration.JsonConfig
{
    public class JsonConfigurationLoader
    {
        public SpecFlowConfiguration LoadJson(SpecFlowConfiguration specFlowConfiguration, string jsonContent)
        {
            if (String.IsNullOrWhiteSpace(jsonContent)) throw new ArgumentNullException(nameof(jsonContent));

            var jsonConfig = JsonConvert.DeserializeObject<JsonConfig>(jsonContent);


            ContainerRegistrationCollection containerRegistrationCollection = specFlowConfiguration.CustomDependencies;
            ContainerRegistrationCollection generatorContainerRegistrationCollection = specFlowConfiguration.GeneratorCustomDependencies;
            CultureInfo featureLanguage = specFlowConfiguration.FeatureLanguage;
            CultureInfo bindingCulture = specFlowConfiguration.BindingCulture;
            string unitTestProvider = specFlowConfiguration.UnitTestProvider;
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

            var specFlowElement = jsonConfig.SpecFlow;
            if (specFlowElement.Language != null)
            {
                if (specFlowElement.Language.Feature.IsNotNullOrWhiteSpace())
                {
                    featureLanguage = CultureInfo.GetCultureInfo(specFlowElement.Language.Feature);
                }
            }

            if (specFlowElement.BindingCulture != null)
            {
                if (specFlowElement.BindingCulture.Name.IsNotNullOrWhiteSpace())
                {
                    bindingCulture = CultureInfo.GetCultureInfo(specFlowElement.BindingCulture.Name);
                }
            }

            if (specFlowElement.UnitTestProvider != null)
            {
                if (specFlowElement.UnitTestProvider.Name.IsNotNullOrWhiteSpace())
                {
                    unitTestProvider = specFlowElement.UnitTestProvider.Name;
                }
            }

            if (specFlowElement.Runtime != null)
            {
                missingOrPendingStepsOutcome = specFlowElement.Runtime.MissingOrPendingStepsOutcome;
                stopAtFirstError = specFlowElement.Runtime.StopAtFirstError;
            }

            if (specFlowElement.Generator != null)
            {
                allowDebugGeneratedFiles = specFlowElement.Generator.AllowDebugGeneratedFiles;
                allowRowTests = specFlowElement.Generator.AllowRowTests;
                markFeaturesParallelizable = specFlowElement.Generator.MarkFeaturesParallelizable;
                skipParallelizableMarkerForTags = specFlowElement.Generator.SkipParallelizableMarkerForTags.ToArray();
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


            return new SpecFlowConfiguration(ConfigSource.Json,
                                            containerRegistrationCollection,
                                            generatorContainerRegistrationCollection,
                                            featureLanguage,
                                            bindingCulture,
                                            unitTestProvider,
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
                                            skipParallelizableMarkerForTags);
        }
    }
}
