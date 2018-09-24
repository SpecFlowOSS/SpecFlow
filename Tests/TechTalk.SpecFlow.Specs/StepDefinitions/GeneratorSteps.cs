using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class GeneratorSteps : Steps
    {
        private readonly SolutionDriver _solutionDriver;

        public GeneratorSteps(SolutionDriver solutionDriver)
        {
            _solutionDriver = solutionDriver;
        }

        [When(@"the feature files in the project are generated")]
        public void WhenTheFeatureFilesInTheProjectAreGenerated()
        {
            _solutionDriver.CompileSolution();
        }

        [Then(@"no generation error is reported")]
        public void ThenNoGenerationErrorIsReported()
        {
            _solutionDriver.CheckSolutionShouldHaveCompiled();
        }
    }
}
