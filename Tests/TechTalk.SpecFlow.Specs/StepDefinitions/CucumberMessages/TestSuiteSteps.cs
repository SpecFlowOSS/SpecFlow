using System.Linq;
using System.Text;
using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.StepDefinitions.CucumberMessages
{
    [Binding]
    public class TestSuiteSteps
    {
        private readonly ProjectsDriver _projectsDriver;

        public TestSuiteSteps(ProjectsDriver projectsDriver)
        {
            _projectsDriver = projectsDriver;
        }

        [Given(@"there are (\d+) feature files")]
        public void GivenThereAreFeatureFiles(int featureFilesCount)
        {
            if (featureFilesCount == 0)
            {
                _projectsDriver.CreateProject("C#");
                return;
            }

            for (int n = 0; n < featureFilesCount; n++)
            {
                string featureTitle = $"Feature{n}";
                var featureBuilder = new StringBuilder();
                featureBuilder.AppendLine($"Feature: {featureTitle}");

                foreach (string scenario in Enumerable.Range(0, 1).Select(i => $"Scenario: passing scenario nr {i}\r\nWhen the step pass in {featureTitle}"))
                {
                    featureBuilder.AppendLine(scenario);
                    featureBuilder.AppendLine();
                }

                _projectsDriver.AddFeatureFile(featureBuilder.ToString());
            }
        }

        [Given(@"the cucumber implementation is (.*)")]
        public void GivenTheCucumberImplementationIs(string cucumberImplementation)
        {
            const string featureTitle = "Feature1";
            var featureBuilder = new StringBuilder();
            featureBuilder.AppendLine($"Feature: {featureTitle}");

            foreach (string scenario in Enumerable.Range(0, 1).Select(i => $"Scenario: passing scenario nr {i}\r\nWhen the step pass in {featureTitle}"))
            {
                featureBuilder.AppendLine(scenario);
                featureBuilder.AppendLine();
            }

            _projectsDriver.AddFeatureFile(featureBuilder.ToString());
        }
    }
}
