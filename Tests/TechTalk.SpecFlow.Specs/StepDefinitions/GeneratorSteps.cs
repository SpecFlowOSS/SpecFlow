using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class GeneratorSteps : Steps
    {
        private readonly CompilationDriver _compilationDriver;
        private readonly CompilationResultDriver _compilationResultDriver;

        public GeneratorSteps(CompilationDriver compilationDriver, CompilationResultDriver compilationResultDriver)
        {
            _compilationDriver = compilationDriver;
            _compilationResultDriver = compilationResultDriver;
        }

        [When(@"the feature files in the project are generated")]
        public void WhenTheFeatureFilesInTheProjectAreGenerated()
        {
            _compilationDriver.CompileSolution();
        }

        [Then(@"no generation error is reported")]
        public void ThenNoGenerationErrorIsReported()
        {
            _compilationResultDriver.CheckSolutionShouldHaveCompiled();
        }
    }
}
