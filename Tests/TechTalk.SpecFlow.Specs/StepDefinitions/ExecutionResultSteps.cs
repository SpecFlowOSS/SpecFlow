using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Specs.Drivers;
using TechTalk.SpecFlow.Assist;
using Should;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class ExecutionResultSteps
    {
        private readonly TestExecutionResult testExecutionResult;

        public ExecutionResultSteps(TestExecutionResult testExecutionResult)
        {
            this.testExecutionResult = testExecutionResult;
        }

        public TestRunSummary ConvertToSummary(Table table)
        {
            return table.CreateInstance<TestRunSummary>();
        }

        [Then(@"all tests should pass")]
        public void ThenAllTestsShouldPass()
        {
            testExecutionResult.LastExecutionSummary.ShouldNotBeNull();
            testExecutionResult.LastExecutionSummary.Succeeded.ShouldEqual(testExecutionResult.LastExecutionSummary.Total);
        }

        [Then(@"the execution summary should contain")]
        public void ThenTheExecutionSummaryShouldContain(Table expectedSummary)
        {
            testExecutionResult.LastExecutionSummary.ShouldNotBeNull();
            expectedSummary.CompareToInstance(testExecutionResult.LastExecutionSummary);
        }

        [Then(@"the binding method '(.*)' is executed")]
        public void ThenTheBindingMethodIsExecuted(string methodName)
        {
            ThenTheBindingMethodIsExecuted(methodName, int.MaxValue);
        }

        [Then(@"the binding method '(.*)' is executed (.*)")]
        public void ThenTheBindingMethodIsExecuted(string methodName, int times)
        {
            testExecutionResult.ExecutionLog.ShouldNotBeNull("no execution log generated");

            var regex = new Regex(@"-> done: \S+\." + methodName);
            if (times > 0)
                regex.Match(testExecutionResult.ExecutionLog).Success.ShouldBeTrue("method " + methodName + " was not executed.");

            if (times != int.MaxValue)
                regex.Matches(testExecutionResult.ExecutionLog).Count.ShouldEqual(times);
        }

        [Then(@"the hook '(.*)' is executed (\D.*)")]
        [Then(@"the hook '(.*)' is executed (\d+) times")]
        public void ThenTheHookIsExecuted(string methodName, int times)
        {
            testExecutionResult.ExecutionLog.ShouldNotBeNull("no execution log generated");

            var regex = new Regex(@"-> hook: " + methodName);
            if (times > 0)
                regex.Match(testExecutionResult.ExecutionLog).Success.ShouldBeTrue("method " + methodName + " was not executed.");

            if (times != int.MaxValue)
                regex.Matches(testExecutionResult.ExecutionLog).Count.ShouldEqual(times);
        }

        [Then(@"the execution log should contain text '(.*)'")]
        public void ThenTheExecutionLogShouldContainText(string text)
        {
            testExecutionResult.ExecutionLog.ShouldNotBeNull("no execution log generated");
            testExecutionResult.ExecutionLog.ShouldContain(text);
        }
    }
}
