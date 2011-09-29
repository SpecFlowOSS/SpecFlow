using System;
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
    }
}
