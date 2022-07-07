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
            var inheritedTags = new[] { "featureTag", "US123" };
            var scenarioTags = new[] { "scenarioTag", "manual" };

            var scenarioInfo = new ScenarioInfo("test scenario", "test_description", scenarioTags, null, inheritedTags);
            
            var combinedTags = scenarioInfo.CombinedTags;
            inheritedTags.All(ft => combinedTags.Contains(ft)).Should().BeTrue();
            scenarioTags.All(st => combinedTags.Contains(st)).Should().BeTrue();
        }
    }
}
