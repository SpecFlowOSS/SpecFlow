using System.Linq;
using FluentAssertions;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests
{
    public class ScenarioInfoTests : StepExecutionTestsBase
    {
        [Fact]
        public void Should_be_able_to_get_scenario_and_feature_level_tags_on_scenario_level()
        {
            var featureTags = new[] { "featureTag", "US123" };
            var scenarioTags = new[] { "scenarioTag", "manual" };

            var scenarioInfo = new ScenarioInfo("test scenario", "test_description", scenarioTags, null, featureTags);
            
            var scenarioAndFeatureTags = scenarioInfo.ScenarioAndFeatureTags;
            featureTags.All(ft => scenarioAndFeatureTags.Contains(ft)).Should().BeTrue();
            scenarioTags.All(st => scenarioAndFeatureTags.Contains(st)).Should().BeTrue();
        }
    }
}
