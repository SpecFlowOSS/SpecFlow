using System;
using System.Linq;
using TechTalk.SpecFlow.Specs.Drivers;
using TechTalk.SpecFlow.Specs.Drivers.MsBuild;
using Should;

namespace TechTalk.SpecFlow.IntegrationTests.StepDefinitions
{
    [Binding]
    public class GeneratorSteps : Steps
    {
        private readonly InputProjectDriver inputProjectDriver;
        private readonly ProjectGenerator projectGenerator;
        private readonly ProjectCompiler projectCompiler;
        private Exception compilationError;

        public GeneratorSteps(InputProjectDriver inputProjectDriver, ProjectGenerator projectGenerator, ProjectCompiler projectCompiler)
        {
            this.inputProjectDriver = inputProjectDriver;
            this.projectCompiler = projectCompiler;
            this.projectGenerator = projectGenerator;
        }

        [When(@"the feature files in the project are generated")]
        public void WhenTheFeatureFilesInTheProjectAreGenerated()
        {
            var project = projectGenerator.GenerateProject(inputProjectDriver);
            try
            {
                compilationError = null;
                projectCompiler.Compile(project, "UpdateFeatureFilesInProject");
            }
            catch (Exception ex)
            {
                compilationError = ex;
            }
        }

        [Then(@"no generation error is reported")]
        public void ThenNoGenerationErrorIsReported()
        {
            compilationError.ShouldBeNull();
        }
    }
}
