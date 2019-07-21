using FluentAssertions;
using System;
using System.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Specs.MSBuild.Drivers;

namespace TechTalk.SpecFlow.Specs.MSBuild.StepDefinitions
{
    [Binding]
    public class GenerationSteps
    {
        private readonly ProjectDriver _projectDriver;

        public GenerationSteps(ProjectDriver projectDriver)
        {
            _projectDriver = projectDriver;
        }

        [Given(@"a project with no features")]
        public void GivenAProjectWithNoFeatures()
        {
            _projectDriver.CreateProject();
        }
        
        [Given(@"a project with these features")]
        public void GivenAProjectWithTheseFeatures(Table table)
        {
            var featureNames = table.Rows.Select(row => row[0]);

            foreach (var title in featureNames)
            {
                _projectDriver.AddFeatureFile(
                $@"Feature: Feature {Guid.NewGuid()}
Scenario: {title}
Given a step");
            }
        }
        
        [Given(@"a project with these features which has been built successfully")]
        public void GivenAProjectWithTheseFeaturesWhichHasBeenBuiltSuccessfully(Table table)
        {
            throw new PendingStepException();
        }
        
        [When(@"the project is built")]
        public void WhenTheProjectIsBuilt()
        {
            _projectDriver.BuildProject();
        }
        
        [When(@"the ""(.*)"" feature is removed")]
        public void WhenTheFeatureIsRemoved(string featureName)
        {
            throw new PendingStepException();
        }
        
        [Then(@"the project should be compiled without errors")]
        public void ThenTheProjectShouldBeCompiledWithoutErrors()
        {
            _projectDriver.BuiltSuccessfully.Should().BeTrue();
        }
        
        [Then(@"the project output should be a test suite for these features")]
        public void ThenTheProjectOutputShouldBeATestSuiteForTheseFeatures(Table table)
        {
            throw new PendingStepException();
        }
    }
}
