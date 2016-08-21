using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoDi;
using Newtonsoft.Json;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Plugins;

namespace TechTalk.SpecFlow.PlatformSpecific.JsonConfig
{
    public class JsonConfigurationLoader
    {
        public RuntimeConfiguration LoadJson(RuntimeConfiguration runtimeConfiguration, string jsonContent)
        {
            if (String.IsNullOrWhiteSpace(jsonContent)) throw new ArgumentNullException(nameof(jsonContent));

            var jsonConfig = JsonConvert.DeserializeObject<PlatformSpecific.JsonConfig.JsonConfig>(jsonContent);


            ContainerRegistrationCollection containerRegistrationCollection = runtimeConfiguration.CustomDependencies;
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
            string generatorPath = runtimeConfiguration.GeneratorPath;
            bool allowRowTests = runtimeConfiguration.AllowRowTests;
            bool allowDebugGeneratedFiles = runtimeConfiguration.AllowDebugGeneratedFiles;


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


            return new RuntimeConfiguration(ConfigSource.Json,
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
