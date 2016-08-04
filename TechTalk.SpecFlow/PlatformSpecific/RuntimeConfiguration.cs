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
    public class RuntimeConfiguration
    {
        public ContainerRegistrationCollection CustomDependencies { get; set;}

        //language settings
        public CultureInfo FeatureLanguage { get; set;}
        public CultureInfo ToolLanguage { get; set;}
        public CultureInfo BindingCulture { get; set;}

        //unit test framework settings
        public string RuntimeUnitTestProvider { get; set;}

        //runtime settings
        public bool DetectAmbiguousMatches { get; set;}
        public bool StopAtFirstError { get; set; }
        public MissingOrPendingStepsOutcome MissingOrPendingStepsOutcome { get; set;}

        //tracing settings
        public bool TraceSuccessfulSteps { get; set;}
        public bool TraceTimings { get; set;}
        public TimeSpan MinTracedDuration { get; set;}
        public StepDefinitionSkeletonStyle StepDefinitionSkeletonStyle { get; set;}

        public List<string> AdditionalStepAssemblies { get; set;}

        public List<PluginDescriptor> Plugins { get; set; }
        

        public RuntimeConfiguration(ContainerRegistrationCollection customDependencies, 
            CultureInfo featureLanguage, 
            CultureInfo toolLanguage, 
            CultureInfo bindingCulture, 
            string runtimeUnitTestProvider, 
            bool detectAmbiguousMatches, 
            bool stopAtFirstError, 
            MissingOrPendingStepsOutcome missingOrPendingStepsOutcome, 
            bool traceSuccessfulSteps, 
            bool traceTimings, 
            TimeSpan minTracedDuration, 
            StepDefinitionSkeletonStyle stepDefinitionSkeletonStyle, 
            List<string> additionalStepAssemblies,
            List<PluginDescriptor> pluginDescriptors)
        {
            CustomDependencies = customDependencies;
            FeatureLanguage = featureLanguage;
            ToolLanguage = toolLanguage;
            BindingCulture = bindingCulture;
            RuntimeUnitTestProvider = runtimeUnitTestProvider;
            DetectAmbiguousMatches = detectAmbiguousMatches;
            StopAtFirstError = stopAtFirstError;
            MissingOrPendingStepsOutcome = missingOrPendingStepsOutcome;
            TraceSuccessfulSteps = traceSuccessfulSteps;
            TraceTimings = traceTimings;
            MinTracedDuration = minTracedDuration;
            StepDefinitionSkeletonStyle = stepDefinitionSkeletonStyle;
            AdditionalStepAssemblies = additionalStepAssemblies;
            Plugins = pluginDescriptors;
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
            return Equals(FeatureLanguage, other.FeatureLanguage) && Equals(ToolLanguage, other.ToolLanguage) && Equals(BindingCulture, other.BindingCulture) && string.Equals(RuntimeUnitTestProvider, other.RuntimeUnitTestProvider) && DetectAmbiguousMatches.Equals(other.DetectAmbiguousMatches) && StopAtFirstError.Equals(other.StopAtFirstError) && MissingOrPendingStepsOutcome.Equals(other.MissingOrPendingStepsOutcome) && TraceSuccessfulSteps.Equals(other.TraceSuccessfulSteps) && TraceTimings.Equals(other.TraceTimings) && MinTracedDuration.Equals(other.MinTracedDuration) && StepDefinitionSkeletonStyle.Equals(other.StepDefinitionSkeletonStyle) &&
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
                hashCode = (hashCode*397) ^ (RuntimeUnitTestProvider != null ? RuntimeUnitTestProvider.GetHashCode() : 0);
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