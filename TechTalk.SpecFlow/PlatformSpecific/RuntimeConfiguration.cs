using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using BoDi;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Compatibility;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.Configuration
{
    public enum ConfigSource
    {
        Default,
        AppConfig,
        Json
    }

    public class RuntimeConfiguration 
    {
        public ConfigSource ConfigSource { get; set; }

        public ContainerRegistrationCollection CustomDependencies { get; set;}
        public ContainerRegistrationCollection GeneratorCustomDependencies { get; set; }


        //language settings
        public CultureInfo FeatureLanguage { get; set;}
        public CultureInfo ToolLanguage { get; set;}
        public CultureInfo BindingCulture { get; set;}

        //unit test framework settings
        public string UnitTestProvider { get; set;}

        //runtime settings
        public bool DetectAmbiguousMatches { get; set;}
        public bool StopAtFirstError { get; set; }
        public MissingOrPendingStepsOutcome MissingOrPendingStepsOutcome { get; set;}

        public bool AllowDebugGeneratedFiles { get; set; }
        public bool AllowRowTests { get; set; }

        //tracing settings
        public bool TraceSuccessfulSteps { get; set;}
        public bool TraceTimings { get; set;}
        public TimeSpan MinTracedDuration { get; set;}
        public StepDefinitionSkeletonStyle StepDefinitionSkeletonStyle { get; set;}

        public List<string> AdditionalStepAssemblies { get; set;}

        public List<PluginDescriptor> Plugins { get; set; }
        public string GeneratorPath { get; set; }


        public RuntimeConfiguration(ConfigSource configSource,
            ContainerRegistrationCollection customDependencies, 
            ContainerRegistrationCollection generatorCustomDependencies,
            CultureInfo featureLanguage, 
            CultureInfo toolLanguage, 
            CultureInfo bindingCulture, 
            string unitTestProvider, 
            bool detectAmbiguousMatches, 
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
            string generatorPath)
        {
            ConfigSource = configSource;
            CustomDependencies = customDependencies;
            GeneratorCustomDependencies = generatorCustomDependencies;
            FeatureLanguage = featureLanguage;
            ToolLanguage = toolLanguage;
            BindingCulture = bindingCulture;
            UnitTestProvider = unitTestProvider;
            DetectAmbiguousMatches = detectAmbiguousMatches;
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
            GeneratorPath = generatorPath;
        }

        public static IEnumerable<PluginDescriptor> GetPlugins()
        {
            var section = (ConfigurationSectionHandler)ConfigurationManager.GetSection("specFlow");
            if (section == null || section.Plugins == null)
                return Enumerable.Empty<PluginDescriptor>();

            return section.Plugins.Select(pce => pce.ToPluginDescriptor());
        }
        

       
       

        #region Equality members

        protected bool Equals(RuntimeConfiguration other)
        {
            return Equals(FeatureLanguage, other.FeatureLanguage) && Equals(ToolLanguage, other.ToolLanguage) && Equals(BindingCulture, other.BindingCulture) && string.Equals(UnitTestProvider, other.UnitTestProvider) && DetectAmbiguousMatches.Equals(other.DetectAmbiguousMatches) && StopAtFirstError.Equals(other.StopAtFirstError) && MissingOrPendingStepsOutcome.Equals(other.MissingOrPendingStepsOutcome) && TraceSuccessfulSteps.Equals(other.TraceSuccessfulSteps) && TraceTimings.Equals(other.TraceTimings) && MinTracedDuration.Equals(other.MinTracedDuration) && StepDefinitionSkeletonStyle.Equals(other.StepDefinitionSkeletonStyle) &&
                (AdditionalStepAssemblies ?? new List<string>()).SequenceEqual(other.AdditionalStepAssemblies ?? new List<string>());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RuntimeConfiguration) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (FeatureLanguage != null ? FeatureLanguage.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ToolLanguage != null ? ToolLanguage.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (BindingCulture != null ? BindingCulture.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (UnitTestProvider != null ? UnitTestProvider.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ DetectAmbiguousMatches.GetHashCode();
                hashCode = (hashCode*397) ^ StopAtFirstError.GetHashCode();
                hashCode = (hashCode*397) ^ MissingOrPendingStepsOutcome.GetHashCode();
                hashCode = (hashCode*397) ^ TraceSuccessfulSteps.GetHashCode();
                hashCode = (hashCode*397) ^ TraceTimings.GetHashCode();
                hashCode = (hashCode*397) ^ MinTracedDuration.GetHashCode();
                hashCode = (hashCode*397) ^ StepDefinitionSkeletonStyle.GetHashCode();
                return hashCode;
            }
        }

        #endregion
    }
}