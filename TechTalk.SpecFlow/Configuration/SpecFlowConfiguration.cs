using System;
using System.Collections.Generic;
using System.Globalization;
using BoDi;
using TechTalk.SpecFlow.BindingSkeletons;

namespace TechTalk.SpecFlow.Configuration
{
    public enum ConfigSource
    {
        AppConfig,
        Json,
        Default
    }

    public class SpecFlowConfiguration
    {
        public SpecFlowConfiguration(ConfigSource configSource,
            ContainerRegistrationCollection customDependencies,
            ContainerRegistrationCollection generatorCustomDependencies,
            CultureInfo featureLanguage,
            CultureInfo bindingCulture,
            bool stopAtFirstError,
            MissingOrPendingStepsOutcome missingOrPendingStepsOutcome,
            bool traceSuccessfulSteps,
            bool traceTimings,
            TimeSpan minTracedDuration,
            StepDefinitionSkeletonStyle stepDefinitionSkeletonStyle,
            List<string> additionalStepAssemblies,
            bool allowDebugGeneratedFiles,
            bool allowRowTests,
            bool markFeaturesParallelizable,
            string[] skipParallelizableMarkerForTags,
            ObsoleteBehavior obsoleteBehavior,
            CucumberMessagesConfiguration cucumberMessagesConfiguration)
        {
            CucumberMessagesConfiguration = cucumberMessagesConfiguration;
            ConfigSource = configSource;
            CustomDependencies = customDependencies;
            GeneratorCustomDependencies = generatorCustomDependencies;
            FeatureLanguage = featureLanguage;
            BindingCulture = bindingCulture;
            StopAtFirstError = stopAtFirstError;
            MissingOrPendingStepsOutcome = missingOrPendingStepsOutcome;
            TraceSuccessfulSteps = traceSuccessfulSteps;
            TraceTimings = traceTimings;
            MinTracedDuration = minTracedDuration;
            StepDefinitionSkeletonStyle = stepDefinitionSkeletonStyle;
            AdditionalStepAssemblies = additionalStepAssemblies;
            AllowDebugGeneratedFiles = allowDebugGeneratedFiles;
            AllowRowTests = allowRowTests;
            MarkFeaturesParallelizable = markFeaturesParallelizable;
            SkipParallelizableMarkerForTags = skipParallelizableMarkerForTags;
            ObsoleteBehavior = obsoleteBehavior;
        }

        public CucumberMessagesConfiguration CucumberMessagesConfiguration { get; }

        public ConfigSource ConfigSource { get; set; }

        public ContainerRegistrationCollection CustomDependencies { get; set; }
        public ContainerRegistrationCollection GeneratorCustomDependencies { get; set; }


        //language settings
        public CultureInfo FeatureLanguage { get; set; }
        public CultureInfo BindingCulture { get; set; }

        //runtime settings
        public bool StopAtFirstError { get; set; }
        public MissingOrPendingStepsOutcome MissingOrPendingStepsOutcome { get; set; }

        public bool AllowDebugGeneratedFiles { get; set; }
        public bool AllowRowTests { get; set; }

        public ObsoleteBehavior ObsoleteBehavior { get; set; }

        //tracing settings
        public bool TraceSuccessfulSteps { get; set; }
        public bool TraceTimings { get; set; }
        public TimeSpan MinTracedDuration { get; set; }
        public StepDefinitionSkeletonStyle StepDefinitionSkeletonStyle { get; set; }

        public List<string> AdditionalStepAssemblies { get; set; }

        public bool MarkFeaturesParallelizable { get; set; }
        public string[] SkipParallelizableMarkerForTags { get; set; }

        protected bool Equals(SpecFlowConfiguration other)
        {
            return Equals(CucumberMessagesConfiguration, other.CucumberMessagesConfiguration) && ConfigSource == other.ConfigSource && Equals(CustomDependencies, other.CustomDependencies) &&
                   Equals(GeneratorCustomDependencies, other.GeneratorCustomDependencies) && Equals(FeatureLanguage, other.FeatureLanguage) && Equals(BindingCulture, other.BindingCulture) &&
                   StopAtFirstError == other.StopAtFirstError && MissingOrPendingStepsOutcome == other.MissingOrPendingStepsOutcome && AllowDebugGeneratedFiles == other.AllowDebugGeneratedFiles &&
                   AllowRowTests == other.AllowRowTests && ObsoleteBehavior == other.ObsoleteBehavior && TraceSuccessfulSteps == other.TraceSuccessfulSteps && TraceTimings == other.TraceTimings &&
                   MinTracedDuration.Equals(other.MinTracedDuration) && StepDefinitionSkeletonStyle == other.StepDefinitionSkeletonStyle &&
                   Equals(AdditionalStepAssemblies, other.AdditionalStepAssemblies) && MarkFeaturesParallelizable == other.MarkFeaturesParallelizable &&
                   Equals(SkipParallelizableMarkerForTags, other.SkipParallelizableMarkerForTags);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((SpecFlowConfiguration) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = CucumberMessagesConfiguration != null ? CucumberMessagesConfiguration.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (int) ConfigSource;
                hashCode = (hashCode * 397) ^ (CustomDependencies != null ? CustomDependencies.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (GeneratorCustomDependencies != null ? GeneratorCustomDependencies.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (FeatureLanguage != null ? FeatureLanguage.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BindingCulture != null ? BindingCulture.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ StopAtFirstError.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) MissingOrPendingStepsOutcome;
                hashCode = (hashCode * 397) ^ AllowDebugGeneratedFiles.GetHashCode();
                hashCode = (hashCode * 397) ^ AllowRowTests.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) ObsoleteBehavior;
                hashCode = (hashCode * 397) ^ TraceSuccessfulSteps.GetHashCode();
                hashCode = (hashCode * 397) ^ TraceTimings.GetHashCode();
                hashCode = (hashCode * 397) ^ MinTracedDuration.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) StepDefinitionSkeletonStyle;
                hashCode = (hashCode * 397) ^ (AdditionalStepAssemblies != null ? AdditionalStepAssemblies.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ MarkFeaturesParallelizable.GetHashCode();
                hashCode = (hashCode * 397) ^ (SkipParallelizableMarkerForTags != null ? SkipParallelizableMarkerForTags.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}