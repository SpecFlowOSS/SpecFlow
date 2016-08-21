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
        public Configuration.SpecFlowConfiguration LoadJson(Configuration.SpecFlowConfiguration specFlowConfiguration, string jsonContent)
        {
            if (String.IsNullOrWhiteSpace(jsonContent)) throw new ArgumentNullException(nameof(jsonContent));

            var jsonConfig = JsonConvert.DeserializeObject<JsonConfig>(jsonContent);


            ContainerRegistrationCollection containerRegistrationCollection = specFlowConfiguration.CustomDependencies;
            ContainerRegistrationCollection generatorContainerRegistrationCollection = specFlowConfiguration.GeneratorCustomDependencies;
            CultureInfo featureLanguage = specFlowConfiguration.FeatureLanguage;
            CultureInfo toolLanguage = specFlowConfiguration.ToolLanguage;
            CultureInfo bindingCulture = specFlowConfiguration.BindingCulture;
            string runtimeUnitTestProvider = specFlowConfiguration.UnitTestProvider;
            bool detectAmbiguousMatches = specFlowConfiguration.DetectAmbiguousMatches;
            bool stopAtFirstError = specFlowConfiguration.StopAtFirstError;
            MissingOrPendingStepsOutcome missingOrPendingStepsOutcome = specFlowConfiguration.MissingOrPendingStepsOutcome;
            bool traceSuccessfulSteps = specFlowConfiguration.TraceSuccessfulSteps;
            bool traceTimings = specFlowConfiguration.TraceTimings;
            TimeSpan minTracedDuration = specFlowConfiguration.MinTracedDuration;
            StepDefinitionSkeletonStyle stepDefinitionSkeletonStyle = specFlowConfiguration.StepDefinitionSkeletonStyle;
            List<string> additionalStepAssemblies = specFlowConfiguration.AdditionalStepAssemblies;
            List<PluginDescriptor> pluginDescriptors = specFlowConfiguration.Plugins;
            string generatorPath = specFlowConfiguration.GeneratorPath;
            bool allowRowTests = specFlowConfiguration.AllowRowTests;
            bool allowDebugGeneratedFiles = specFlowConfiguration.AllowDebugGeneratedFiles;


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

            if (specFlowElement.Generator != null)
            {
                
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


            return new Configuration.SpecFlowConfiguration(ConfigSource.Json,
                                            containerRegistrationCollection,
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
                                            generatorPath);
        }
    }
}
