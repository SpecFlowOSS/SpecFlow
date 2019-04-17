using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class FeatureFileSteps
    {
        private readonly ProjectsDriver _projectsDriver;

        public FeatureFileSteps(ProjectsDriver projectsDriver)
        {
            _projectsDriver = projectsDriver;
        }

        [Given(@"there is a feature file in the project as")]
        public void GivenThereIsAFeatureFileInTheProjectAs(string featureFileContent)
        {
            _projectsDriver.AddFeatureFile(featureFileContent);
        }

        [Given(@"a scenario '(.*)' as")]
        public void GivenAScenarioSimpleScenarioAs(string title, string scenarioContent)
        {
            _projectsDriver.AddFeatureFile(
                $@"Feature: Feature {Guid.NewGuid()}
Scenario: {title}
{scenarioContent.Replace("'''", "\"\"\"")}
                ");
        }

        [Given(@"a scenario named '(.*)' with collection tag '(.*)' as")]
        public void GivenAScenarioAsWithCollectionAttribute(string title,  string collection, string scenarioContent)
        {
            _projectsDriver.AddFeatureFile(
                 $@"@xunit:collection({collection})
Feature: Feature {Guid.NewGuid()}
Scenario: {title}
                    {scenarioContent.Replace("'''", "\"\"\"")}
                ");
        }

        [Given(@"there is a scenario in a feature file")]
        public void GivenThereIsAScenarioInAFeatureFile()
        {
            GivenAScenarioSimpleScenarioAs("Scenario", "Given a step");
        }

        [Given(@"has a feature file with the SingleFileGenerator configured")]
        public void GivenHasAFeatureFileWithTheSingleFileGeneratorConfigured()
        {
            _projectsDriver.AddFile("FeatureWithSingleFileGenerator.feature", "Feature: Feature File with SingleFileGenerator", "None", new Dictionary<string, string>(){{"Generator", "SpecFlowSingleFileGenerator"}});
        }




        [Given(@"there is a feature '(.*)' with (\d+) passing (\d+) failing (\d+) pending and (\d+) ignored scenarios")]
        public void GivenThereAreScenarios(string featureTitle, int passCount, int failCount, int pendingCount, int ignoredCount)
        {
            StringBuilder featureBuilder = new StringBuilder();
            featureBuilder.AppendLine("Feature: " + featureTitle);

            foreach (var scenario in Enumerable.Range(0, passCount).Select(i => string.Format("Scenario: passing scenario nr {0}\r\nWhen the step pass in " + featureTitle, i)))
            {
                featureBuilder.AppendLine(scenario);
                featureBuilder.AppendLine();
            }

            foreach (var scenario in Enumerable.Range(0, failCount).Select(i => string.Format("Scenario: failing scenario nr {0}\r\nWhen the step fail in " + featureTitle, i)))
            {
                featureBuilder.AppendLine(scenario);
                featureBuilder.AppendLine();
            }

            foreach (var scenario in Enumerable.Range(0, pendingCount).Select(i => $"Scenario: pending scenario nr {i}\r\nWhen the step is pending"))
            {
                featureBuilder.AppendLine(scenario);
                featureBuilder.AppendLine();
            }

            foreach (var scenario in Enumerable.Range(0, ignoredCount).Select(i => $"@ignore\r\nScenario: ignored scenario nr {i}\r\nWhen the step is ignored"))
            {
                featureBuilder.AppendLine(scenario);
                featureBuilder.AppendLine();
            }

            _projectsDriver.AddFeatureFile(featureBuilder.ToString());

            _projectsDriver.AddStepBinding(ScenarioBlock.When.ToString(), "the step pass in " + featureTitle, "//pass", "'pass");
            _projectsDriver.AddStepBinding(ScenarioBlock.When.ToString(), "the step fail in " + featureTitle, "throw new System.Exception(\"simulated failure\");", "Throw New System.Exception(\"simulated failure\")");
        }
    }
}
