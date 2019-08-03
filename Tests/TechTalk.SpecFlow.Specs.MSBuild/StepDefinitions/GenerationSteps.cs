using FluentAssertions;
using System;
using System.Linq;
using TechTalk.SpecFlow.Specs.MSBuild.Support;
using TechTalk.SpecFlow.TestProjectGenerator;
using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.MSBuild.StepDefinitions
{
    [Binding]
    public class GenerationSteps
    {
        private readonly ProjectsDriver _projectsDriver;
        private readonly SolutionDriver _solutionDriver;
        private readonly TestProjectFolders _testProjectFolders;

        public GenerationSteps(
            ProjectsDriver projectsDriver,
            SolutionDriver solutionDriver,
            TestProjectFolders testProjectFolders)
        {
            _projectsDriver = projectsDriver;
            _solutionDriver = solutionDriver;
            _testProjectFolders = testProjectFolders;
        }

        [Given(@"a project with no features")]
        public void GivenAProjectWithNoFeatures()
        {
        }
        
        [Given(@"a project with these features")]
        public void GivenAProjectWithTheseFeatures(Table table)
        {
            var featureNames = table.Rows.Select(row => row[0]);

            foreach (var title in featureNames)
            {
                _projectsDriver.AddFeatureFile(
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
            _solutionDriver.CompileSolution(BuildTool.DotnetBuild);
        }
        
        [When(@"the ""(.*)"" feature is removed")]
        public void WhenTheFeatureIsRemoved(string featureName)
        {
            throw new PendingStepException();
        }
        
        [Then(@"the project should have been compiled without errors")]
        public void ThenTheProjectShouldHaveBeenCompiledWithoutErrors()
        {
            _solutionDriver.CheckSolutionShouldHaveCompiled();
        }
        
        [Then(@"the project output should be a test suite for these features")]
        public void ThenTheProjectOutputShouldBeATestSuiteForTheseFeatures(Table table)
        {
            var featureNames = table.Rows.Select(row => row[0]);

            var assembly = TestAssemblyInfo.ReadTestAssembly(_testProjectFolders.CompiledAssemblyPath);

            assembly.Features.Should().BeEquivalentTo(featureNames.Select(f => new TestFeatureInfo { Title = f }));
        }
    }
}
