using System;
using System.IO;
using System.Linq;
using BoDi;
using FluentAssertions;
using Moq;
using Xunit;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Configuration.AppConfig;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.RuntimeTests.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.RuntimeTests.Configuration
{
    
    public class AppConfigTests
    {
        [Fact]
        public void CanLoadConfigFromConfigFile()
        {
            var specFlowJsonLocatorMock = new Mock<ISpecFlowJsonLocator>();

            var runtimeConfiguration = ConfigurationLoader.GetDefault();
            var configurationLoader = new ConfigurationLoader(specFlowJsonLocatorMock.Object);

            runtimeConfiguration = configurationLoader.Load(runtimeConfiguration);
        }

        [Theory]
        [InlineData(@"<specFlow>
    <language feature=""en"" tool=""en"" /> 
    
    <generator allowDebugGeneratedFiles=""false"" />
    
    <runtime stopAtFirstError=""false""
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

        [Fact]
        public void CheckFeatureLanguage()
        {
            string config = @"<specflow><language feature=""de"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.FeatureLanguage.TwoLetterISOLanguageName.Should().Be("de");
        }


        [Fact]
        public void CheckBindingCulture()
        {
            string config = @"<specflow><bindingCulture name=""de"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.BindingCulture.TwoLetterISOLanguageName.Should().Be("de");
        }

        [Fact]
        public void Check_Runtime_stopAtFirstError_as_true()
        {
            string config = @"<specflow><runtime stopAtFirstError=""true"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.StopAtFirstError.Should().BeTrue();
        }

        [Fact]
        public void Check_Runtime_stopAtFirstError_as_false()
        {
            string config = @"<specflow><runtime stopAtFirstError=""false"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.StopAtFirstError.Should().BeFalse();
        }

        [Fact]
        public void Check_Runtime_missingOrPendingStepsOutcome_as_Pending()
        {
            string config = @"<specflow><runtime missingOrPendingStepsOutcome=""Pending"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.MissingOrPendingStepsOutcome.Should().Be(MissingOrPendingStepsOutcome.Pending);
        }

        [Fact]
        public void Check_Runtime_missingOrPendingStepsOutcome_as_Error()
        {
            string config = @"<specflow><runtime missingOrPendingStepsOutcome=""Error"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.MissingOrPendingStepsOutcome.Should().Be(MissingOrPendingStepsOutcome.Error);
        }

        [Fact]
        public void Check_Runtime_missingOrPendingStepsOutcome_as_Ignore()
        {
            string config = @"<specflow><runtime missingOrPendingStepsOutcome=""Ignore"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.MissingOrPendingStepsOutcome.Should().Be(MissingOrPendingStepsOutcome.Ignore);
        }

        [Fact]
        public void Check_Runtime_missingOrPendingStepsOutcome_as_Inconclusive()
        {
            string config = @"<specflow><runtime missingOrPendingStepsOutcome=""Inconclusive"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.MissingOrPendingStepsOutcome.Should().Be(MissingOrPendingStepsOutcome.Inconclusive);
        }

        [Fact]
        public void Check_Trace_traceSuccessfulSteps_as_True()
        {
            string config = @"<specflow><trace traceSuccessfulSteps=""true"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.TraceSuccessfulSteps.Should().BeTrue();
        }

        [Fact]
        public void Check_Trace_traceSuccessfulSteps_as_False()
        {
            string config = @"<specflow><trace traceSuccessfulSteps=""false"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.TraceSuccessfulSteps.Should().BeFalse();
        }

        [Fact]
        public void Check_Trace_traceTimings_as_True()
        {
            string config = @"<specflow><trace traceTimings=""true"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.TraceTimings.Should().BeTrue();
        }

        [Fact]
        public void Check_Trace_traceTimings_as_False()
        {
            string config = @"<specflow><trace traceTimings=""false"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.TraceTimings.Should().BeFalse();
        }

        [Fact]
        public void Check_Trace_minTracedDuration()
        {
            string config = @"<specflow><trace minTracedDuration=""0:0:1.0"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.MinTracedDuration.Should().Be(TimeSpan.FromSeconds(1));
        }

        [Fact]
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

        [Fact]
        public void Check_Trace_StepDefinitionSkeletonStyle_RegexAttribute()
        {
            string config = @"<specflow><trace stepDefinitionSkeletonStyle=""RegexAttribute"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.StepDefinitionSkeletonStyle.Should().Be(StepDefinitionSkeletonStyle.RegexAttribute);
        }

        [Fact]
        public void Check_Trace_StepDefinitionSkeletonStyle_MethodNamePascalCase()
        {
            string config = @"<specflow><trace stepDefinitionSkeletonStyle=""MethodNamePascalCase"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.StepDefinitionSkeletonStyle.Should().Be(StepDefinitionSkeletonStyle.MethodNamePascalCase);
        }

        [Fact]
        public void Check_Trace_StepDefinitionSkeletonStyle_MethodNameRegex()
        {
            string config = @"<specflow><trace stepDefinitionSkeletonStyle=""MethodNameRegex"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.StepDefinitionSkeletonStyle.Should().Be(StepDefinitionSkeletonStyle.MethodNameRegex);
        }

        [Fact]
        public void Check_Trace_StepDefinitionSkeletonStyle_MethodNameUnderscores()
        {
            string config = @"<specflow><trace stepDefinitionSkeletonStyle=""MethodNameUnderscores"" /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.StepDefinitionSkeletonStyle.Should().Be(StepDefinitionSkeletonStyle.MethodNameUnderscores);
        }

        [Fact]
        public void Check_StepAssemblies_IsEmpty()
        {
            string config = @"<specflow><stepAssemblies /></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.AdditionalStepAssemblies.Should().BeEmpty();
        }

        [Fact]
        public void Check_StepAssemblies_NotInConfigFile()
        {
            string config = @"<specflow></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.AdditionalStepAssemblies.Should().BeEmpty();
        }

        [Fact]
        public void Check_StepAssemblies_OneEntry()
        {
            string config = @"<specflow><stepAssemblies><stepAssembly assembly=""testEntry""/></stepAssemblies></specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            runtimeConfig.AdditionalStepAssemblies.Count.Should().Be(1);
            runtimeConfig.AdditionalStepAssemblies.First().Should().Be("testEntry");
        }

        [Fact]
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


        [Fact]
        public void Check_CucumberMessages_NotConfigured_EnabledIsFalse()
        {
            string config = @"<specflow>
                            </specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);
            runtimeConfig.CucumberMessagesConfiguration.Enabled.Should().BeFalse();
        }


        [Fact]
        public void Check_CucumberMessages_EmptyTag_EnabledIsFalse()
        {
            string config = @"<specflow>
                                <cucumber-messages />
                            </specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);
            runtimeConfig.CucumberMessagesConfiguration.Enabled.Should().BeFalse();
        }

        [Fact]
        public void Check_CucumberMessages_Enabled_True()
        {
            string config = @"<specflow>
                                <cucumber-messages enabled=""true""/>
                            </specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);
            runtimeConfig.CucumberMessagesConfiguration.Enabled.Should().BeTrue();
        }

        [Fact]
        public void Check_CucumberMessages_Enabled_False()
        {
            string config = @"<specflow>
                                <cucumber-messages enabled=""false""/>
                            </specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);
            runtimeConfig.CucumberMessagesConfiguration.Enabled.Should().BeFalse();
        }

        [Fact]
        public void Check_CucumberMessages_Sinks_EmptyList()
        {
            string config = @"<specflow>
                                <cucumber-messages enabled=""false"">
                                    <sinks>
                                    </sinks>
                                </cucumber-messages>
                            </specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);
            runtimeConfig.CucumberMessagesConfiguration.Sinks.Should().BeEmpty();
        }

        [Fact]
        public void Check_CucumberMessages_Sinks_ListOneEntry()
        {
            string config = @"<specflow>
                                <cucumber-messages enabled=""false"">
                                    <sinks>
                                        <sink type=""file"" path=""C:\temp\testrun.cm"" />
                                    </sinks>
                                </cucumber-messages>
                            </specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);
            runtimeConfig.CucumberMessagesConfiguration.Sinks.Count.Should().Be(1);
        }

        [Fact]
        public void Check_CucumberMessages_Sinks_ListMultipleEntry()
        {
            string config = @"<specflow>
                                <cucumber-messages enabled=""false"">
                                    <sinks>
                                        <sink type=""file"" path=""C:\temp\testrun1.cm"" />
                                        <sink type=""file"" path=""C:\temp\testrun2.cm"" />
                                        <sink type=""file"" path=""C:\temp\testrun3.cm"" />
                                    </sinks>
                                </cucumber-messages>
                            </specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);
            runtimeConfig.CucumberMessagesConfiguration.Sinks.Count.Should().Be(3);
        }

        [Fact]
        public void Check_CucumberMessages_Sinks_DataOfEntry()
        {
            string config = @"<specflow>
                                <cucumber-messages enabled=""false"">
                                    <sinks>
                                        <sink type=""file"" path=""C:\temp\testrun.cm"" />
                                    </sinks>
                                </cucumber-messages>
                            </specflow>";

            var configSection = ConfigurationSectionHandler.CreateFromXml(config);

            var runtimeConfig = new AppConfigConfigurationLoader().LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);
            var cucumberMessagesSink = runtimeConfig.CucumberMessagesConfiguration.Sinks.First();

            cucumberMessagesSink.Type.Should().Be("file");
            cucumberMessagesSink.Path.Should().Be(@"C:\temp\testrun.cm");
        }
    }
}
