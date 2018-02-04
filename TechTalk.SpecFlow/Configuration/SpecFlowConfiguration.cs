using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using BoDi;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Configuration.AppConfig;
using TechTalk.SpecFlow.Plugins;

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
            string unitTestProvider,
            bool stopAtFirstError,
            MissingOrPendingStepsOutcome missingOrPendingStepsOutcome,
            bool traceSuccessfulSteps,
            bool traceTimings,
            TimeSpan minTracedDuration,
            StepDefinitionSkeletonStyle stepDefinitionSkeletonStyle,
            List<string> additionalStepAssemblies,
            List<PluginDescriptor> pluginDescriptors,
            bool allowDebugGeneratedFiles,
            bool allowRowTests,
            bool markFeaturesParallelizable,
            string[] skipParallelizableMarkerForTags)
        {
            ConfigSource = configSource;
            CustomDependencies = customDependencies;
            GeneratorCustomDependencies = generatorCustomDependencies;
            FeatureLanguage = featureLanguage;
            BindingCulture = bindingCulture;
            UnitTestProvider = unitTestProvider;
            StopAtFirstError = stopAtFirstError;
            MissingOrPendingStepsOutcome = missingOrPendingStepsOutcome;
            TraceSuccessfulSteps = traceSuccessfulSteps;
            TraceTimings = traceTimings;
            MinTracedDuration = minTracedDuration;
            StepDefinitionSkeletonStyle = stepDefinitionSkeletonStyle;
            AdditionalStepAssemblies = additionalStepAssemblies;
            Plugins = pluginDescriptors;
            AllowDebugGeneratedFiles = allowDebugGeneratedFiles;
            AllowRowTests = allowRowTests;
            MarkFeaturesParallelizable = markFeaturesParallelizable;
            SkipParallelizableMarkerForTags = skipParallelizableMarkerForTags;
        }

        public ConfigSource ConfigSource { get; set; }

        public ContainerRegistrationCollection CustomDependencies { get; set; }
        public ContainerRegistrationCollection GeneratorCustomDependencies { get; set; }


        //language settings
        public CultureInfo FeatureLanguage { get; set; }
        public CultureInfo BindingCulture { get; set; }

        //unit test framework settings
        public string UnitTestProvider { get; set; }

        //runtime settings
        public bool StopAtFirstError { get; set; }
        public MissingOrPendingStepsOutcome MissingOrPendingStepsOutcome { get; set; }

        public bool AllowDebugGeneratedFiles { get; set; }
        public bool AllowRowTests { get; set; }

        //tracing settings
        public bool TraceSuccessfulSteps { get; set; }
        public bool TraceTimings { get; set; }
        public TimeSpan MinTracedDuration { get; set; }
        public StepDefinitionSkeletonStyle StepDefinitionSkeletonStyle { get; set; }

        public List<string> AdditionalStepAssemblies { get; set; }

        public List<PluginDescriptor> Plugins { get; set; }

        public bool MarkFeaturesParallelizable { get; set; }
        public string[] SkipParallelizableMarkerForTags { get; set; }

        public static IEnumerable<PluginDescriptor> GetPlugins()
        {
            var section = (ConfigurationSectionHandler) ConfigurationManager.GetSection("specFlow");
            if (section == null || section.Plugins == null)
                return Enumerable.Empty<PluginDescriptor>();

            return section.Plugins.Select(pce => pce.ToPluginDescriptor());
        }


        protected bool Equals(SpecFlowConfiguration other)
        {
            return ConfigSource == other.ConfigSource && Equals(CustomDependencies, other.CustomDependencies) && Equals(GeneratorCustomDependencies, other.GeneratorCustomDependencies) && Equals(FeatureLanguage, other.FeatureLanguage) && Equals(BindingCulture, other.BindingCulture) && string.Equals(UnitTestProvider, other.UnitTestProvider) && StopAtFirstError == other.StopAtFirstError && MissingOrPendingStepsOutcome == other.MissingOrPendingStepsOutcome && AllowDebugGeneratedFiles == other.AllowDebugGeneratedFiles && AllowRowTests == other.AllowRowTests && TraceSuccessfulSteps == other.TraceSuccessfulSteps && TraceTimings == other.TraceTimings && MinTracedDuration.Equals(other.MinTracedDuration) && StepDefinitionSkeletonStyle == other.StepDefinitionSkeletonStyle && Equals(AdditionalStepAssemblies, other.AdditionalStepAssemblies) && Equals(Plugins, other.Plugins) && MarkFeaturesParallelizable == other.MarkFeaturesParallelizable && Equals(SkipParallelizableMarkerForTags, other.SkipParallelizableMarkerForTags);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SpecFlowConfiguration) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) ConfigSource;
                hashCode = (hashCode * 397) ^ (CustomDependencies != null ? CustomDependencies.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (GeneratorCustomDependencies != null ? GeneratorCustomDependencies.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (FeatureLanguage != null ? FeatureLanguage.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BindingCulture != null ? BindingCulture.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (UnitTestProvider != null ? UnitTestProvider.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ StopAtFirstError.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) MissingOrPendingStepsOutcome;
                hashCode = (hashCode * 397) ^ AllowDebugGeneratedFiles.GetHashCode();
                hashCode = (hashCode * 397) ^ AllowRowTests.GetHashCode();
                hashCode = (hashCode * 397) ^ TraceSuccessfulSteps.GetHashCode();
                hashCode = (hashCode * 397) ^ TraceTimings.GetHashCode();
                hashCode = (hashCode * 397) ^ MinTracedDuration.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) StepDefinitionSkeletonStyle;
                hashCode = (hashCode * 397) ^ (AdditionalStepAssemblies != null ? AdditionalStepAssemblies.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Plugins != null ? Plugins.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ MarkFeaturesParallelizable.GetHashCode();
                hashCode = (hashCode * 397) ^ (SkipParallelizableMarkerForTags != null ? SkipParallelizableMarkerForTags.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}