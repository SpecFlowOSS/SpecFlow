using System;
using System.IO;
using System.Linq;
using BoDi;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Configuration.AppConfig;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.RuntimeTests.Configuration
{
    [TestFixture]
    public class AppConfigTests
    {
        [Test]
        public void CanLoadConfigFromConfigFile()
        {
            var runtimeConfiguration = ConfigurationLoader.GetDefault();
            var configurationLoader = new ConfigurationLoader();

            runtimeConfiguration = configurationLoader.Load(runtimeConfiguration);
        }

        [Test]
        [TestCase(@"<specFlow>
    <language feature=""en"" tool=""en"" /> 

    <unitTestProvider name=""NUnit"" 
                      generatorProvider=""TechTalk.SpecFlow.TestFrameworkIntegration.NUnitRuntimeProvider, TechTalk.SpecFlow""
                      runtimeProvider=""TechTalk.SpecFlow.UnitTestProvider.NUnitRuntimeProvider, TechTalk.SpecFlow"" />

    <generator allowDebugGeneratedFiles=""false"" />
    
    <runtime detectAmbiguousMatches=""true""
             stopAtFirstError=""false""
             missingOrPendingStepsOutcome=""Inconclusive"" />

    <trace traceSuccessfulSteps=""true""
           traceTimings=""false""
           minTracedDuration=""0:0:0.1""
           listener=""TechTalk.SpecFlow.Tracing.DefaultListener, TechTalk.SpecFlow""
            />
</specFlow>")]
        public void CanLoadConfigFromString(string configString)
        {
            var runtimeConfig = ConfigurationLoader.GetDefault();

            var configurationLoader = new AppConfigConfigurationLoader();


            var configurationSectionHandler = ConfigurationSectionHandler.CreateFromXml(configString);
            configurationLoader.LoadAppConfig(runtimeConfig, configurationSectionHandler);
        }

        [Test]
        public void CheckFeatureLanguage()
        {
            string config = @"<specflow><language feature=""de"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.FeatureLanguage.TwoLetterISOLanguageName.Should().Be("de");
        }
        

        [Test]
        public void CheckBindingCulture()
        {
            string config = @"<specflow><bindingCulture name=""de"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.BindingCulture.TwoLetterISOLanguageName.Should().Be("de");
        }

        [Test]
        public void CheckUnitTestProvider()
        {
            string config = @"<specflow><unitTestProvider name=""XUnit"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.UnitTestProvider.Should().Be("XUnit");
        }


        [Test]
        public void Check_Runtime_stopAtFirstError_as_true()
        {
            string config = @"<specflow><runtime stopAtFirstError=""true"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.StopAtFirstError.Should().BeTrue();
        }

        [Test]
        public void Check_Runtime_stopAtFirstError_as_false()
        {
            string config = @"<specflow><runtime stopAtFirstError=""false"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.StopAtFirstError.Should().BeFalse();
        }

        [Test]
        public void Check_Runtime_missingOrPendingStepsOutcome_as_Pending()
        {
            string config = @"<specflow><runtime missingOrPendingStepsOutcome=""Pending"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.MissingOrPendingStepsOutcome.Should().Be(MissingOrPendingStepsOutcome.Pending);
        }

        [Test]
        public void Check_Runtime_missingOrPendingStepsOutcome_as_Error()
        {
            string config = @"<specflow><runtime missingOrPendingStepsOutcome=""Error"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.MissingOrPendingStepsOutcome.Should().Be(MissingOrPendingStepsOutcome.Error);
        }

        [Test]
        public void Check_Runtime_missingOrPendingStepsOutcome_as_Ignore()
        {
            string config = @"<specflow><runtime missingOrPendingStepsOutcome=""Ignore"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.MissingOrPendingStepsOutcome.Should().Be(MissingOrPendingStepsOutcome.Ignore);
        }

        [Test]
        public void Check_Runtime_missingOrPendingStepsOutcome_as_Inconclusive()
        {
            string config = @"<specflow><runtime missingOrPendingStepsOutcome=""Inconclusive"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.MissingOrPendingStepsOutcome.Should().Be(MissingOrPendingStepsOutcome.Inconclusive);
        }

        [Test]
        public void Check_Trace_traceSuccessfulSteps_as_True()
        {
            string config = @"<specflow><trace traceSuccessfulSteps=""true"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.TraceSuccessfulSteps.Should().BeTrue();
        }

        [Test]
        public void Check_Trace_traceSuccessfulSteps_as_False()
        {
            string config = @"<specflow><trace traceSuccessfulSteps=""false"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.TraceSuccessfulSteps.Should().BeFalse();
        }

        [Test]
        public void Check_Trace_traceTimings_as_True()
        {
            string config = @"<specflow><trace traceTimings=""true"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.TraceTimings.Should().BeTrue();
        }

        [Test]
        public void Check_Trace_traceTimings_as_False()
        {
            string config = @"<specflow><trace traceTimings=""false"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.TraceTimings.Should().BeFalse();
        }

        [Test]
        public void Check_Trace_minTracedDuration()
        {
            string config = @"<specflow><trace minTracedDuration=""0:0:1.0"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.MinTracedDuration.Should().Be(TimeSpan.FromSeconds(1));
        }

        [Test]
        public void Check_Trace_Listener()
        {
            string config = @"<specflow><trace listener=""TraceListener"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.CustomDependencies.Count.Should().Be(1);

            foreach (ContainerRegistrationConfigElement containerRegistrationConfigElement in runtimeConfig.CustomDependencies)
            {
                containerRegistrationConfigElement.Implementation.Should().Be("TraceListener");
            }
        }

        [Test]
        public void Check_Trace_StepDefinitionSkeletonStyle_RegexAttribute()
        {
            string config = @"<specflow><trace stepDefinitionSkeletonStyle=""RegexAttribute"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.StepDefinitionSkeletonStyle.Should().Be(StepDefinitionSkeletonStyle.RegexAttribute);
        }

        [Test]
        public void Check_Trace_StepDefinitionSkeletonStyle_MethodNamePascalCase()
        {
            string config = @"<specflow><trace stepDefinitionSkeletonStyle=""MethodNamePascalCase"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.StepDefinitionSkeletonStyle.Should().Be(StepDefinitionSkeletonStyle.MethodNamePascalCase);
        }

        [Test]
        public void Check_Trace_StepDefinitionSkeletonStyle_MethodNameRegex()
        {
            string config = @"<specflow><trace stepDefinitionSkeletonStyle=""MethodNameRegex"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.StepDefinitionSkeletonStyle.Should().Be(StepDefinitionSkeletonStyle.MethodNameRegex);
        }

        [Test]
        public void Check_Trace_StepDefinitionSkeletonStyle_MethodNameUnderscores()
        {
            string config = @"<specflow><trace stepDefinitionSkeletonStyle=""MethodNameUnderscores"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.StepDefinitionSkeletonStyle.Should().Be(StepDefinitionSkeletonStyle.MethodNameUnderscores);
        }

        [Test]
        public void Check_StepAssemblies_IsEmpty()
        {
            string config = @"<specflow><stepAssemblies /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.AdditionalStepAssemblies.Should().BeEmpty();
        }

        [Test]
        public void Check_StepAssemblies_NotInConfigFile()
        {
            string config = @"<specflow></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.AdditionalStepAssemblies.Should().BeEmpty();
        }

        [Test]
        public void Check_StepAssemblies_OneEntry()
        {
            string config = @"<specflow><stepAssemblies><stepAssembly assembly=""testEntry""/></stepAssemblies></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.AdditionalStepAssemblies.Count.Should().Be(1);
            runtimeConfig.AdditionalStepAssemblies.First().Should().Be("testEntry");
        }

        [Test]
        public void Check_StepAssemblies_TwoEntry()
        {
            string config = @"<specflow><stepAssemblies>
                                <stepAssembly assembly=""testEntry1""/>
                                <stepAssembly assembly=""testEntry2""/>
                              </stepAssemblies></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.AdditionalStepAssemblies.Count.Should().Be(2);
            runtimeConfig.AdditionalStepAssemblies[0].Should().Be("testEntry1");
            runtimeConfig.AdditionalStepAssemblies[1].Should().Be("testEntry2");
        }

        [Test]
        public void Check_Plugins_IsEmpty()
        {
            string config = @"<specflow><plugins /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.Plugins.Should().BeEmpty();
        }

        [Test]
        public void Check_Plugins_NotInConfigFile()
        {
            string config = @"<specflow></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.Plugins.Should().BeEmpty();
        }

        [Test]
        public void Check_Plugins_OneEntry()
        {
            string config = @"<specflow><plugins><add name=""testEntry""/></plugins></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.Plugins.Count.Should().Be(1);
            runtimeConfig.Plugins.First().Name.Should().Be("testEntry");
        }

        [Test]
        public void Check_Plugins_PluginPath()
        {
            string config = @"<specflow><plugins><add name=""testEntry"" path=""path_to_assembly""/></plugins></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.Plugins.Count.Should().Be(1);
            runtimeConfig.Plugins.First().Path.Should().Be("path_to_assembly");
        }

        [Test]
        public void Check_Plugins_Parameters()
        {
            string config = @"<specflow><plugins><add name=""testEntry"" parameters=""pluginParameter""/></plugins></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.Plugins.Count.Should().Be(1);
            runtimeConfig.Plugins.First().Parameters.Should().Be("pluginParameter");
        }

        [Test]
        public void Check_Plugins_PluginType_Runtime()
        {
            string config = @"<specflow><plugins><add name=""testEntry"" type=""Runtime""/></plugins></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.Plugins.Count.Should().Be(1);
            runtimeConfig.Plugins.First().Type.Should().Be(PluginType.Runtime);
        }

        [Test]
        public void Check_Plugins_PluginType_Generator()
        {
            string config = @"<specflow><plugins><add name=""testEntry"" type=""Generator""/></plugins></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.Plugins.Count.Should().Be(1);
            runtimeConfig.Plugins.First().Type.Should().Be(PluginType.Generator);
        }

        [Test]
        public void Check_Plugins_PluginType_GeneratorAndRuntime()
        {
            string config = @"<specflow><plugins><add name=""testEntry"" type=""GeneratorAndRuntime""/></plugins></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.Plugins.Count.Should().Be(1);
            runtimeConfig.Plugins.First().Type.Should().Be(PluginType.GeneratorAndRuntime);
        }

        [Test]
        public void Check_Plugins_TwoEntry()
        {
            string config = @"<specflow><plugins>
                                <add name=""testEntry1""/>
                                <add name=""testEntry2""/>
                              </plugins></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.Plugins.Count.Should().Be(2);
            runtimeConfig.Plugins[0].Name.Should().Be("testEntry1");
            runtimeConfig.Plugins[1].Name.Should().Be("testEntry2");
        }
    }
}
