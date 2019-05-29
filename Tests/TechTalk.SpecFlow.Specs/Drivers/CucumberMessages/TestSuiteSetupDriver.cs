using System.Linq;
using System.Text;
using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.Drivers.CucumberMessages
{
    public class TestSuiteSetupDriver
    {
        private readonly ProjectsDriver _projectsDriver;
        private bool _isProjectCreated;

        public TestSuiteSetupDriver(ProjectsDriver projectsDriver)
        {
            _projectsDriver = projectsDriver;
        }

        public void AddFeatureFiles(int count)
        {
            if (count <= 0 && !_isProjectCreated)
            {
                _projectsDriver.CreateProject("C#");
                _isProjectCreated = true;
                return;
            }

            for (int n = 0; n < count; n++)
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

            _isProjectCreated = true;
        }

        public void EnsureAProjectIsCreated()
        {
            if (_isProjectCreated)
            {
                return;
            }

            AddFeatureFiles(1);
        }
    }
}
