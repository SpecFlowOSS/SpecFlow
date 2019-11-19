using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class GeneratorSteps : Steps
    {
        private readonly CompilationDriver _compilationDriver;

        public GeneratorSteps(CompilationDriver compilationDriver)
        {
            _compilationDriver = compilationDriver;
        }

        [When(@"the feature files in the project are generated")]
        public void WhenTheFeatureFilesInTheProjectAreGenerated()
        {
            _compilationDriver.CompileSolution();
        }

        [Then(@"no generation error is reported")]
        public void ThenNoGenerationErrorIsReported()
        {
            _compilationDriver.CheckSolutionShouldHaveCompiled();
        }
    }
}
