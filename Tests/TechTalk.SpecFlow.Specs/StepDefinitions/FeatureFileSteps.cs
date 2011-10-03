using System;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Specs.Drivers;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class FeatureFileSteps
    {
        private readonly InputProjectDriver inputProjectDriver;

        public FeatureFileSteps(InputProjectDriver inputProjectDriver)
        {
            this.inputProjectDriver = inputProjectDriver;
        }

        [Given(@"there is a feature file in the project as")]
        public void GivenThereIsAFeatureFileInTheProjectAs(string featureFileContent)
        {
            inputProjectDriver.AddFeatureFile(featureFileContent);
        }

        [Given(@"a scenario '(.*)' as")]
        public void GivenAScenarioSimpleScenarioAs(string title, string scenarioContent)
        {

            inputProjectDriver.AddFeatureFile(string.Format(@"Feature: Feature {0}
Scenario: {1}
{2}
                ", Guid.NewGuid(), title, scenarioContent));
        }

        [Given(@"there is a feature '(.*)' with (\d+) passing (\d+) failing (\d+) pending and (\d+) ignored scenarios")]
        public void GivenThereAreScenarios(string featureTitle, int passCount, int failCount, int pendingCount, int ignoredCount)
        {
            StringBuilder featureBuilder = new StringBuilder();
            featureBuilder.AppendLine("Feature: " + featureTitle);

            foreach (var scenario in Enumerable.Range(0, passCount).Select(
                i => string.Format("Scenario: passing scenario nr {0}\r\nWhen the step pass in " + featureTitle, i)))
            {
                featureBuilder.AppendLine(scenario);
                featureBuilder.AppendLine();
            }

            foreach (var scenario in Enumerable.Range(0, failCount).Select(
                i => string.Format("Scenario: failing scenario nr {0}\r\nWhen the step fail in " + featureTitle, i)))
            {
                featureBuilder.AppendLine(scenario);
                featureBuilder.AppendLine();
            }

            foreach (var scenario in Enumerable.Range(0, pendingCount).Select(
                i => string.Format("Scenario: pending scenario nr {0}\r\nWhen the step is pending", i)))
            {
                featureBuilder.AppendLine(scenario);
                featureBuilder.AppendLine();
            }

            foreach (var scenario in Enumerable.Range(0, ignoredCount).Select(
                i => string.Format("@ignore\r\nScenario: ignored scenario nr {0}\r\nWhen the step is ignored", i)))
            {
                featureBuilder.AppendLine(scenario);
                featureBuilder.AppendLine();
            }

            inputProjectDriver.AddFeatureFile(featureBuilder.ToString());

            inputProjectDriver.AddStepBinding(ScenarioBlock.When, "the step pass in " + featureTitle, code: "//pass");
            inputProjectDriver.AddStepBinding(ScenarioBlock.When, "the step fail in " + featureTitle, code: "throw new System.Exception(\"simulated failure\");");
        }
    }
}
