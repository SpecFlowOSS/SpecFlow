using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Configuration.AppConfig;
using TechTalk.SpecFlow.Generator.Configuration;

namespace TechTalk.SpecFlow.GeneratorTests
{
    
    public class ConfigurationTests
    {
        private const string ConfigWithParallelCodeGenerationOptions =
            @"<specFlow>
                <language feature=""en"" tool=""en"" /> 

                <unitTestProvider name=""NUnit"" 
                                    generatorProvider=""TechTalk.SpecFlow.TestFrameworkIntegration.NUnitRuntimeProvider, TechTalk.SpecFlow""
                                    runtimeProvider=""TechTalk.SpecFlow.UnitTestProvider.NUnitRuntimeProvider, TechTalk.SpecFlow"" />

                <generator allowDebugGeneratedFiles=""false""
                           markFeaturesParallelizable=""true"">
                    <skipParallelizableMarkerForTags>
                        <tag value=""mySpecialTag1""/>
                        <tag value=""mySpecialTag2""/>
                    </skipParallelizableMarkerForTags>
                </generator>
                 
    
                <runtime detectAmbiguousMatches=""true""
                            stopAtFirstError=""false""
                            missingOrPendingStepsOutcome=""Inconclusive"" />

                <trace traceSuccessfulSteps=""true""
                        traceTimings=""false""
                        minTracedDuration=""0:0:0.1""
                        listener=""TechTalk.SpecFlow.Tracing.DefaultListener, TechTalk.SpecFlow""
                        />
            </specFlow>";

        [Fact]
        public void CanLoadConfigWithParallelCodeGenerationOptionsFromString()
        {
            var specFlowConfiguration = ConfigurationLoader.GetDefault();

            var configurationLoader = new AppConfigConfigurationLoader();


            var configurationSectionHandler = ConfigurationSectionHandler.CreateFromXml(ConfigWithParallelCodeGenerationOptions);
            specFlowConfiguration = configurationLoader.LoadAppConfig(specFlowConfiguration, configurationSectionHandler);



            specFlowConfiguration.MarkFeaturesParallelizable.Should().BeTrue();
            specFlowConfiguration.SkipParallelizableMarkerForTags.Should().NotBeEmpty();
            Assert.Contains("mySpecialTag1", specFlowConfiguration.SkipParallelizableMarkerForTags);
            Assert.Contains("mySpecialTag2", specFlowConfiguration.SkipParallelizableMarkerForTags);
        }
    }
}