using System;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.Drivers.CucumberMessages
{
    public class TestSuiteSetupDriver
    {
        private readonly ProjectsDriver _projectsDriver;
        private readonly TestSuiteInitializationDriver _testSuiteInitializationDriver;
        private bool _isProjectCreated;

        public TestSuiteSetupDriver(ProjectsDriver projectsDriver, TestSuiteInitializationDriver testSuiteInitializationDriver)
        {
            _projectsDriver = projectsDriver;
            _testSuiteInitializationDriver = testSuiteInitializationDriver;
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

        public void AddScenarios(int scenariosCount)
        {
            if (scenariosCount <= 0 && !_isProjectCreated)
            {
                _projectsDriver.CreateProject("C#");
                _isProjectCreated = true;
                return;
            }

            const string featureTitle = "Feature1";
            var featureBuilder = new StringBuilder();
            featureBuilder.AppendLine($"Feature: {featureTitle}");

            foreach (string scenario in Enumerable.Range(0, scenariosCount).Select(i => $"Scenario: passing scenario nr {i}\r\nWhen the step pass in {featureTitle}"))
            {
                featureBuilder.AppendLine(scenario);
                featureBuilder.AppendLine();
            }

            _projectsDriver.AddFeatureFile(featureBuilder.ToString());

            _isProjectCreated = true;
        }

        public void AddScenario(Guid pickleId)
        {
            AddScenarios(1);
            _testSuiteInitializationDriver.OverrideTestCaseStartedPickleId = pickleId;
            _testSuiteInitializationDriver.OverrideTestCaseFinishedPickleId = pickleId;
        }

        public void AddDuplicateStepDefinition()
        {
            _projectsDriver.AddStepBinding("When", "the step pass in .*", "//pass", "'pass");
            _projectsDriver.AddStepBinding("When", "the step pass in .*", "//pass", "'pass");
        }

        public void AddNotMatchingStepDefinition()
        {
            _projectsDriver.AddStepBinding("When", "the step does not pass in .*", "//pass", "'pass");
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
