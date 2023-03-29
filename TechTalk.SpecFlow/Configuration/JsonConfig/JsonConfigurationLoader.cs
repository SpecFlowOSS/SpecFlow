using System;
using System.Globalization;
using SpecFlow.Internal.Json;

namespace TechTalk.SpecFlow.Configuration.JsonConfig
{
    public class JsonConfigurationLoader
    {
        public SpecFlowConfiguration LoadJson(SpecFlowConfiguration specFlowConfiguration, string jsonContent)
        {
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                throw new ArgumentNullException(nameof(jsonContent));
            }

            var jsonConfig = jsonContent.FromJson<JsonConfig>();

            var containerRegistrationCollection = specFlowConfiguration.CustomDependencies;
            var generatorContainerRegistrationCollection = specFlowConfiguration.GeneratorCustomDependencies;
            var featureLanguage = specFlowConfiguration.FeatureLanguage;
            var bindingCulture = specFlowConfiguration.BindingCulture;
            bool stopAtFirstError = specFlowConfiguration.StopAtFirstError;
            var missingOrPendingStepsOutcome = specFlowConfiguration.MissingOrPendingStepsOutcome;
            bool traceSuccessfulSteps = specFlowConfiguration.TraceSuccessfulSteps;
            bool traceTimings = specFlowConfiguration.TraceTimings;
            var minTracedDuration = specFlowConfiguration.MinTracedDuration;
            bool coloredOutput = specFlowConfiguration.ColoredOutput;
            var stepDefinitionSkeletonStyle = specFlowConfiguration.StepDefinitionSkeletonStyle;
            var additionalStepAssemblies = specFlowConfiguration.AdditionalStepAssemblies;
            bool allowRowTests = specFlowConfiguration.AllowRowTests;
            bool allowDebugGeneratedFiles = specFlowConfiguration.AllowDebugGeneratedFiles;
            var addNonParallelizableMarkerForTags = specFlowConfiguration.AddNonParallelizableMarkerForTags;
            var obsoleteBehavior = specFlowConfiguration.ObsoleteBehavior;

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
                addNonParallelizableMarkerForTags = jsonConfig.Generator.AddNonParallelizableMarkerForTags?.ToArray();
            }

            if (jsonConfig.Trace != null)
            {
                traceSuccessfulSteps = jsonConfig.Trace.TraceSuccessfulSteps;
                traceTimings = jsonConfig.Trace.TraceTimings;
                minTracedDuration = jsonConfig.Trace.MinTracedDuration;
                stepDefinitionSkeletonStyle = jsonConfig.Trace.StepDefinitionSkeletonStyle;
                coloredOutput = jsonConfig.Trace.ColoredOutput;
            }

            if (jsonConfig.StepAssemblies != null)
            {
                foreach (var stepAssemblyEntry in jsonConfig.StepAssemblies)
                {
                    additionalStepAssemblies.Add(stepAssemblyEntry.Assembly);
                }
            }

            return new SpecFlowConfiguration(
                ConfigSource.Json,
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
                addNonParallelizableMarkerForTags,
                obsoleteBehavior,
                coloredOutput
            );
        }
    }
}
