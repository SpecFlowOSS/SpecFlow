using System;
using System.Collections.Generic;
using System.Linq;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class TestRunResultCollector : ITestRunResultCollector
    {
        private readonly IDictionary<ScenarioInfo, TestResult> _collectedResults = new Dictionary<ScenarioInfo, TestResult>();
        public bool IsStarted { get; private set; }

        public void StartCollecting()
        {
            if (IsStarted)
            {
                throw new InvalidOperationException("Result collection has already been started.");
            }

            _collectedResults.Clear();
            IsStarted = true;
        }

        public void CollectTestResultForScenario(ScenarioInfo scenarioInfo, TestResult testResult)
        {
            if (!IsStarted)
            {
                throw new InvalidOperationException("Result collection has not been started.");
            }

            _collectedResults.Add(scenarioInfo, testResult);
        }

        public IResult<TestRunResult> StopCollecting()
        {
            if (!IsStarted)
            {
                return Result<TestRunResult>.Failure(new InvalidOperationException("Result collection has not been started."));
            }

            var groups = _collectedResults.GroupBy(kv => kv.Value.Status, kv => (kv.Key, kv.Value))
                                          .ToArray();

            int passedCount = groups.Single(g => g.Key == TestResult.Types.Status.Passed).Count();
            int failedCount = groups.Single(g => g.Key == TestResult.Types.Status.Failed).Count();
            int skippedCount = groups.Single(g => g.Key == TestResult.Types.Status.Skipped).Count();
            int ambiguousCount = groups.Single(g => g.Key == TestResult.Types.Status.Ambiguous).Count();
            int undefinedCount = groups.Single(g => g.Key == TestResult.Types.Status.Undefined).Count();

            var testRunResult = new TestRunResult(
                _collectedResults.Count,
                passedCount,
                failedCount,
                skippedCount,
                ambiguousCount,
                undefinedCount);

            IsStarted = false;
            _collectedResults.Clear();

            return Result.Success(testRunResult);
        }
    }
}
