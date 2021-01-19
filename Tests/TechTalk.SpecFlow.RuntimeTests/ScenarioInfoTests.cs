using System.Linq;
using BoDi;
using FluentAssertions;
using Moq;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.RuntimeTests.Infrastructure;
using TechTalk.SpecFlow.Tracing;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests
{
    public class ScenarioInfoTests : StepExecutionTestsBase
    {
        private ContextManager _contextManager;

        [Fact]
        public void Should_be_able_to_get_scenario_and_feature_level_tags_on_scenario_level()
        {
            var featureTags = new[] { "featureTag", "US123" };
            var scenarioTags = new[] { "scenarioTag", "manual" };

            InitializeContexts(featureTags, scenarioTags);

            var scenarioAndFeatureTags = _contextManager.ScenarioContext.ScenarioInfo.ScenarioAndFeatureTags;
            featureTags.All(ft => scenarioAndFeatureTags.Contains(ft)).Should().BeTrue();
            scenarioTags.All(st => scenarioAndFeatureTags.Contains(st)).Should().BeTrue();
        }

        private void InitializeContexts(string[] featureTags, string[] scenarioTags)
        {
            var containerBuilder = new RuntimeTestsContainerBuilder();
            var testThreadContainer = containerBuilder.CreateTestThreadContainer(containerBuilder.CreateGlobalContainer(typeof(TestThreadContextTests).Assembly));
            _contextManager = CreateContextManager(testThreadContainer);

            _contextManager.InitializeFeatureContext(new FeatureInfo(FeatureLanguage, "", "test feature", null, featureTags));
            _contextManager.InitializeScenarioContext(new ScenarioInfo("test scenario", "test_description", scenarioTags, null, featureTags));
        }

        private ContextManager CreateContextManager(IObjectContainer testThreadContainer = null)
        {
            return new ContextManager(new Mock<ITestTracer>().Object, testThreadContainer ?? TestThreadContainer, ContainerBuilderStub);
        }
    }
}
