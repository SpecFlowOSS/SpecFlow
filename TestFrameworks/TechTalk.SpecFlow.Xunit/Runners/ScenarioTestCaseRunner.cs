﻿using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Gherkin.Ast;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Xunit.TestArtifacts;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TechTalk.SpecFlow.Xunit.Runners
{
    public class ScenarioTestCaseRunner : TestCaseRunner<ScenarioTestCase>
    {
        private ITestRunner _testRunner;

        public ScenarioTestCaseRunner(ScenarioTestCase testCase, IMessageBus messageBus, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource) 
            : base(testCase, messageBus, aggregator, cancellationTokenSource)
        {
        }

        public void FeatureSetup(GherkinDocument gherkinDocument)
        {
            Debug.Assert(gherkinDocument.Feature != null);
            var feature = gherkinDocument.Feature;

            var assembly = Assembly.LoadFrom(TestCase.FeatureFile.SpecFlowProject.AssemblyPath);
            _testRunner = TestRunnerManager.GetTestRunner(assembly);
            var featureInfo = new FeatureInfo(GetFeatureCulture(feature.Language), feature.Name, feature.Description, ProgrammingLanguage.CSharp, feature.Tags.GetTags().ToArray());
            _testRunner.OnFeatureStart(featureInfo);
        }

        private CultureInfo GetFeatureCulture(string language)
        {
            var culture = new CultureInfo(language);
            if (culture.IsNeutralCulture)
                return new CultureInfo("en-US"); //TODO: find the "default" specific culture for the neutral culture, like 'en-US' for 'en'. This is currently in the SpecFlow generator

            return culture;
        }

        public void FeatureTearDown()
        {
            _testRunner.OnFeatureEnd();
            _testRunner = null;
        }

        public void ScenarioTearDown()
        {
            _testRunner.OnScenarioEnd();
        }

        public virtual void ScenarioSetup(ScenarioInfo scenarioInfo, TestOutputHelper outputHelper)
        {
            _testRunner.OnScenarioStart(scenarioInfo, container =>
            {
                container.RegisterInstanceAs(outputHelper, typeof(ITestOutputHelper));
            });
        }

        public virtual void ScenarioCleanup()
        {
            _testRunner.CollectScenarioErrors();
        }


        protected override async Task<RunSummary> RunTestAsync()
        {
            var test = new XunitTest(TestCase,
                TestCase.DisplayName); //TODO: this is a pickle, we could use the Compiler/Pickle interfaces from the Gherkin parser
            var summary = new RunSummary {Total = 1};
            var output = new StringBuilder();

            var outputHelper = new TestOutputHelper();
            outputHelper.Initialize(MessageBus, test);

            var gherkinDocument =
                await SpecFlowParserHelper.ParseSpecFlowDocumentAsync(TestCase.FeatureFile.FeatureFilePath);

            Scenario scenario = null;
            if (gherkinDocument.SpecFlowFeature != null)
                if (TestCase.IsScenarioOutline)
                {
                    var scenarioOutline = gherkinDocument.SpecFlowFeature.ScenarioDefinitions.OfType<ScenarioOutline>()
                        .FirstOrDefault(s => s.Name == TestCase.Name);
                    Examples example = null;
                    Gherkin.Ast.TableRow exampleRow = null;
                    if (scenarioOutline != null && SpecFlowParserHelper.GetExampleRowById(scenarioOutline,
                            TestCase.ExampleId, out example, out exampleRow))
                        scenario = SpecFlowParserHelper.CreateScenario(scenarioOutline, example, exampleRow);
                }
                else
                {
                    scenario = gherkinDocument.SpecFlowFeature.ScenarioDefinitions.OfType<Scenario>()
                        .FirstOrDefault(s => s.Name == TestCase.Name);
                }

            string skipReason = null;
            if (scenario == null)
                skipReason = $"Unable to find Scenario: {TestCase.DisplayName}";
            else if (gherkinDocument.SpecFlowFeature.Tags.GetTags().Concat(scenario.Tags.GetTags()).Contains("ignore"))
                skipReason = "Ignored";

            if (skipReason != null)
            {
                summary.Skipped++;

                if (!MessageBus.QueueMessage(new TestSkipped(test, skipReason)))
                    CancellationTokenSource.Cancel();
            }
            else
            {
                var aggregator = new ExceptionAggregator(Aggregator);
                if (!aggregator.HasExceptions)
                    aggregator.Run(() => RunScenario(gherkinDocument, scenario, output, outputHelper));

                var exception = aggregator.ToException();
                TestResultMessage testResult;
                if (exception == null)
                {
                    testResult = new TestPassed(test, summary.Time, output.ToString());
                }
                else
                {
                    testResult = new TestFailed(test, summary.Time, output.ToString(), exception);
                    summary.Failed++;
                }

                if (!CancellationTokenSource.IsCancellationRequested)
                    if (!MessageBus.QueueMessage(testResult))
                        CancellationTokenSource.Cancel();
            }

            if (!MessageBus.QueueMessage(new TestFinished(test, summary.Time, output.ToString())))
                CancellationTokenSource.Cancel();

            return summary;
        }

        private void RunScenario(SpecFlowDocument gherkinDocument, Scenario scenario, StringBuilder output, TestOutputHelper outputHelper)
        {
            FeatureSetup(gherkinDocument);

            var scenarioInfo = new ScenarioInfo(scenario.Name, scenario.Tags.GetTags().ToArray());
            ScenarioSetup(scenarioInfo, outputHelper);

            var steps = scenario.Steps.Cast<SpecFlowStep>();
            if (gherkinDocument.SpecFlowFeature.Background != null)
                steps = gherkinDocument.SpecFlowFeature.Background.Steps.Cast<SpecFlowStep>().Concat(steps);

            try
            {
                foreach (var step in steps)
                {
                    output.AppendLine($"> Running {step.Keyword}{step.Text}");
                    ExecuteStep(step);
                }

                ScenarioCleanup(); // normally this is the point when the scenario errors are thrown
            }
            finally
            {
                ScenarioTearDown();
                FeatureTearDown();
            }
        }

        private void ExecuteStep(SpecFlowStep step)
        {
            var docStringArg = step.Argument as DocString;
            var docString = docStringArg?.Content;
            var dataTableArg = step.Argument as DataTable;
            Table dataTable = null;
            if (dataTableArg != null && dataTableArg.Rows.Any())
            {
                dataTable = new Table(dataTableArg.Rows.First().Cells.Select(c => c.Value).ToArray());
                foreach (var row in dataTableArg.Rows.Skip(1))
                    dataTable.AddRow(row.Cells.Select(c => c.Value).ToArray());
            }
            switch (step.StepKeyword)
            {
                case StepKeyword.Given:
                    _testRunner.Given(step.Text, docString, dataTable, step.Keyword);
                    break;
                case StepKeyword.When:
                    _testRunner.When(step.Text, docString, dataTable, step.Keyword);
                    break;
                case StepKeyword.Then:
                    _testRunner.Then(step.Text, docString, dataTable, step.Keyword);
                    break;
                case StepKeyword.And:
                    _testRunner.And(step.Text, docString, dataTable, step.Keyword);
                    break;
                case StepKeyword.But:
                    _testRunner.But(step.Text, docString, dataTable, step.Keyword);
                    break;
            }
        }
    }
}