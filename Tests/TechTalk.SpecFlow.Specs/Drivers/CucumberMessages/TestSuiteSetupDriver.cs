using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public void AddGenericWhenStepBinding()
        {
            _projectsDriver.AddStepBinding("When", ".*", "//pass", "'pass");
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

        public void AddScenarioWithGivenStep(string step)
        {
            if (!_isProjectCreated)
            {
                _projectsDriver.CreateProject("C#");
                _isProjectCreated = true;
            }

            const string featureTitle = "Feature1";
            var featureBuilder = new StringBuilder();
            featureBuilder.AppendLine($"Feature: {featureTitle}");

            featureBuilder.AppendLine("Scenario: scenario");
            featureBuilder.AppendLine($"Given {step}");
            featureBuilder.AppendLine();

            _projectsDriver.AddFeatureFile(featureBuilder.ToString());

            _isProjectCreated = true;
        }

        public void AddStepDefinitionsFromStringList(string stepDefinitionOrder)
        {
            if (!_isProjectCreated)
            {
                _projectsDriver.CreateProject("C#");
                _isProjectCreated = true;
            }

            var order = ParseOrderFromString(stepDefinitionOrder);
            foreach (var stepDefinitionRow in order.StepDefinitionRows)
            {
                (string csharp, string vbnet) = GetStepDefinitionCodeForExecution(stepDefinitionRow.Execution);
                _projectsDriver.AddStepBinding("Given", stepDefinitionRow.Name, csharp, vbnet);
            }
        }

        public StepDefinitionOrder ParseOrderFromString(string stepDefinitionOrder)
        {
            var regex = new Regex(@"(?<BindingName>[A-Za-z0-9_]+) \((?<Result>pass|fail|pending)\)");
            var matches = regex.Matches(stepDefinitionOrder).Cast<Match>();
            var stepDefinitionRows = from m in matches
                                     let bindingName = m.Groups["BindingName"].Value
                                     let name = bindingName.EndsWith("Binding") ? bindingName.Substring(0, bindingName.Length - "Binding".Length) : bindingName
                                     let resultString = m.Groups["Result"].Value
                                     let result = (StepDefinitionRowExecution)Enum.Parse(typeof(StepDefinitionRowExecution), resultString, true)
                                     select new StepDefinitionRow { Name = name, Execution = result };
            return new StepDefinitionOrder
            {
                StepDefinitionRows = stepDefinitionRows.ToList()
            };
        }

        public (string csharp, string vbnet) GetStepDefinitionCodeForExecution(StepDefinitionRowExecution execution)
        {
            switch (execution)
            {
                case StepDefinitionRowExecution.Pass: return ("//pass", "'pass");
                case StepDefinitionRowExecution.Fail: return (@"throw new global::System.Exception(""Expected failure"");", @"Throw New System.Exception(""Expected failure"")");
                case StepDefinitionRowExecution.Pending: return (@"ScenarioContext.Current.Pending();", @"ScenarioContext.Current.Pending()");
                default: throw new NotSupportedException($"Not supported {nameof(StepDefinitionRowExecution)}: {execution}");
            }
        }
    }

    public class StepDefinitionOrder
    {
        public List<StepDefinitionRow> StepDefinitionRows { get; set; } = new List<StepDefinitionRow>();
    }

    public class StepDefinitionRow
    {
        public StepDefinitionRowExecution Execution { get; set; }
        public string Name { get; set; }
    }

    public enum StepDefinitionRowExecution
    {
        Pass,
        Fail,
        Pending
    }
}
