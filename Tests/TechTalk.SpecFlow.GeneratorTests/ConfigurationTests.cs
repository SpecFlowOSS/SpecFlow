using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Configuration.AppConfig;

namespace TechTalk.SpecFlow.GeneratorTests
{

    public class ConfigurationTests
    {
        private const string ConfigWithParallelCodeGenerationOptions =
            @"<specFlow>
                <language feature=""en"" tool=""en"" /> 
                
                <generator allowDebugGeneratedFiles=""false""
                           markFeaturesParallelizable=""true"">
                    <skipParallelizableMarkerForTags>
                        <tag value=""mySpecialTag1""/>
                        <tag value=""mySpecialTag2""/>
                    </skipParallelizableMarkerForTags>
                </generator>
                 
    
                <runtime stopAtFirstError=""false""
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

        [Fact]
        public void CanLoadConfigWithNonParallelizableTagsProvided()
        {
            var config =
                @"<specFlow>
                    <generator>
                        <addNonParallelizableMarkerForTags>
                            <tag value=""tag1""/>
                            <tag value=""tag2""/>
                        </addNonParallelizableMarkerForTags>
                    </generator>
                </specFlow>";
            var specFlowConfiguration = ConfigurationLoader.GetDefault();
            var configurationLoader = new AppConfigConfigurationLoader();
            
            var configurationSectionHandler = ConfigurationSectionHandler.CreateFromXml(config);
            specFlowConfiguration = configurationLoader.LoadAppConfig(specFlowConfiguration, configurationSectionHandler);

            specFlowConfiguration.AddNonParallelizableMarkerForTags.Should().BeEquivalentTo("tag1", "tag2");
        }
    }
}