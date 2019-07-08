using System;
using System.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace TechTalk.SpecFlow.Specs.MSBuild.StepDefinitions
{
    [Binding]
    public class GenerationSteps
    {
        [Given(@"a project with no features")]
        public void GivenAProjectWithNoFeatures()
        {
            throw new PendingStepException();
        }
        
        [Given(@"a project with these features")]
        public void GivenAProjectWithTheseFeatures(Table table)
        {
            var featureNames = table.Rows.Select(row => row[0]);
        }
        
        [Given(@"a project with these features which has been built successfully")]
        public void GivenAProjectWithTheseFeaturesWhichHasBeenBuiltSuccessfully(Table table)
        {
            throw new PendingStepException();
        }
        
        [When(@"the project is built")]
        public void WhenTheProjectIsBuilt()
        {
            throw new PendingStepException();
        }
        
        [When(@"the ""(.*)"" feature is removed")]
        public void WhenTheFeatureIsRemoved(string featureName)
        {
            throw new PendingStepException();
        }
        
        [Then(@"the project should be compiled without errors")]
        public void ThenTheProjectShouldBeCompiledWithoutErrors()
        {
            throw new PendingStepException();
        }
        
        [Then(@"the project output should be a test suite for these features")]
        public void ThenTheProjectOutputShouldBeATestSuiteForTheseFeatures(Table table)
        {
            throw new PendingStepException();
        }
    }
}
