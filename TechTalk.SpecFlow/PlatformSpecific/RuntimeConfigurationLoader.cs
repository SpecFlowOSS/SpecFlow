using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoDi;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Compatibility;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.PlatformSpecific
{
    public class RuntimeConfigurationLoader
    {
        private static readonly CultureInfo _defaultFeatureLanguage = CultureInfo.GetCultureInfo(ConfigDefaults.FeatureLanguage);
        private static readonly CultureInfo _defaultToolLanguage = CultureInfoHelper.GetCultureInfo(ConfigDefaults.FeatureLanguage);
        private static readonly CultureInfo _defaultBindingCulture = null;
        private static readonly string _defaultRuntimeUnitTestProvider = ConfigDefaults.UnitTestProviderName;
        private static readonly bool _defaultDetectAmbiguousMatches = ConfigDefaults.DetectAmbiguousMatches;
        private static readonly bool _defaultStopAtFirstError = ConfigDefaults.StopAtFirstError;
        private static readonly MissingOrPendingStepsOutcome _defaultMissingOrPendingStepsOutcome = ConfigDefaults.MissingOrPendingStepsOutcome;
        private static readonly bool _defaultTraceSuccessfulSteps = ConfigDefaults.TraceSuccessfulSteps;
        private static readonly bool _defaultTraceTimings = ConfigDefaults.TraceTimings;
        private static readonly TimeSpan _defaultMinTracedDuration = TimeSpan.Parse(ConfigDefaults.MinTracedDuration);
        private static readonly StepDefinitionSkeletonStyle _defaultStepDefinitionSkeletonStyle = ConfigDefaults.StepDefinitionSkeletonStyle;
        private static readonly List<string> _defaultAdditionalStepAssemblies = new List<string>();

        public RuntimeConfiguration Load(RuntimeConfiguration runtimeConfiguration)
        {
            if (HasJsonConfig)
            {
                return LoadJson();
            }

            if (HasAppConfig)
            {
                return LoadAppConfig(runtimeConfiguration);
            }

            return GetDefault();
        }

        public static RuntimeConfiguration GetDefault()
        {
            return new RuntimeConfiguration(new ContainerRegistrationCollection(), _defaultFeatureLanguage,
                _defaultToolLanguage, _defaultBindingCulture, _defaultRuntimeUnitTestProvider,
                _defaultDetectAmbiguousMatches, _defaultStopAtFirstError, _defaultMissingOrPendingStepsOutcome,
                _defaultTraceSuccessfulSteps, _defaultTraceTimings, _defaultMinTracedDuration,
                _defaultStepDefinitionSkeletonStyle, _defaultAdditionalStepAssemblies);
        }

        private RuntimeConfiguration LoadAppConfig(RuntimeConfiguration runtimeConfiguration)
        {
            var configSection = ConfigurationManager.GetSection("specFlow") as ConfigurationSectionHandler;

            return LoadAppConfig(runtimeConfiguration, configSection);
        }

        public RuntimeConfiguration LoadAppConfig(RuntimeConfiguration runtimeConfiguration,
            ConfigurationSectionHandler configSection)
        {
            if (configSection == null) throw new ArgumentNullException("configSection");

            ContainerRegistrationCollection containerRegistrationCollection = runtimeConfiguration.CustomDependencies;
            CultureInfo featureLanguage = runtimeConfiguration.FeatureLanguage;
            CultureInfo toolLanguage = runtimeConfiguration.ToolLanguage;
            CultureInfo bindingCulture = runtimeConfiguration.BindingCulture;
            string runtimeUnitTestProvider = runtimeConfiguration.RuntimeUnitTestProvider;
            bool detectAmbiguousMatches = runtimeConfiguration.DetectAmbiguousMatches;
            bool stopAtFirstError = runtimeConfiguration.StopAtFirstError;
            MissingOrPendingStepsOutcome missingOrPendingStepsOutcome = runtimeConfiguration.MissingOrPendingStepsOutcome;
            bool traceSuccessfulSteps = runtimeConfiguration.TraceSuccessfulSteps;
            bool traceTimings = runtimeConfiguration.TraceTimings;
            TimeSpan minTracedDuration = runtimeConfiguration.MinTracedDuration;
            StepDefinitionSkeletonStyle stepDefinitionSkeletonStyle = runtimeConfiguration.StepDefinitionSkeletonStyle;
            List<string> additionalStepAssemblies = runtimeConfiguration.AdditionalStepAssemblies;


            if (IsSpecified(configSection.Language))
            {
                featureLanguage = CultureInfo.GetCultureInfo(configSection.Language.Feature);
                toolLanguage = string.IsNullOrEmpty(configSection.Language.Tool)
                    ? CultureInfo.GetCultureInfo(configSection.Language.Feature)
                    : CultureInfo.GetCultureInfo(configSection.Language.Tool);
            }

            if (IsSpecified(configSection.BindingCulture))
            {
                bindingCulture = CultureInfo.GetCultureInfo(configSection.BindingCulture.Name);
            }

            if (IsSpecified(configSection.Runtime))
            {
                detectAmbiguousMatches = configSection.Runtime.DetectAmbiguousMatches;


                stopAtFirstError = configSection.Runtime.StopAtFirstError;
                missingOrPendingStepsOutcome = configSection.Runtime.MissingOrPendingStepsOutcome;

                if (IsSpecified(configSection.Runtime.Dependencies))
                {
                    containerRegistrationCollection = configSection.Runtime.Dependencies;
                }
            }

            if (IsSpecified(configSection.UnitTestProvider))
            {
                if (!string.IsNullOrEmpty(configSection.UnitTestProvider.RuntimeProvider))
                {
                    //compatibility mode, we simulate a custom dependency
                    runtimeUnitTestProvider = "custom";
                    containerRegistrationCollection.Add(configSection.UnitTestProvider.RuntimeProvider,
                        typeof(IUnitTestRuntimeProvider).AssemblyQualifiedName, runtimeUnitTestProvider);
                }
                else
                {
                    runtimeUnitTestProvider = configSection.UnitTestProvider.Name;
                }
            }


            if (IsSpecified(configSection.Trace))
            {
                if (!string.IsNullOrEmpty(configSection.Trace.Listener)) // backwards compatibility
                {
                    containerRegistrationCollection.Add(configSection.Trace.Listener,
                        typeof(ITraceListener).AssemblyQualifiedName);
                }

                traceSuccessfulSteps = configSection.Trace.TraceSuccessfulSteps;
                traceTimings = configSection.Trace.TraceTimings;
                minTracedDuration = configSection.Trace.MinTracedDuration;
                stepDefinitionSkeletonStyle = configSection.Trace.StepDefinitionSkeletonStyle;
            }

            foreach (var element in configSection.StepAssemblies)
            {
                var assemblyName = ((StepAssemblyConfigElement) element).Assembly;
                additionalStepAssemblies.Add(assemblyName);
            }

            return new RuntimeConfiguration(containerRegistrationCollection, featureLanguage, toolLanguage, bindingCulture,
                runtimeUnitTestProvider, detectAmbiguousMatches, stopAtFirstError, missingOrPendingStepsOutcome,
                traceSuccessfulSteps, traceTimings, minTracedDuration, stepDefinitionSkeletonStyle, additionalStepAssemblies);
        }


        private RuntimeConfiguration LoadJson()
        {
            throw new NotImplementedException();
        }

        public bool HasAppConfig => ConfigurationManager.GetSection("specFlow") != null;

        public bool HasJsonConfig => false;

        private bool IsSpecified(ConfigurationElement configurationElement)
        {
            return configurationElement != null && configurationElement.ElementInformation.IsPresent;
        }
    }
}
