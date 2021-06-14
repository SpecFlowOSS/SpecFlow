using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Configuration.AppConfig;

namespace TechTalk.SpecFlow.GeneratorTests
{

    public class ConfigurationTests
    {
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