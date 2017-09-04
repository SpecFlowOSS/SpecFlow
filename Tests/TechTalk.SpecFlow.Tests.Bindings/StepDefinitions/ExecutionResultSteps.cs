using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FluentAssertions;
using TechTalk.SpecFlow.Specs.Drivers;
using TechTalk.SpecFlow.Assist;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class ExecutionResultSteps
    { 
        private readonly TestExecutionResult testExecutionResult;
        private readonly HooksDriver hooksDriver;

        public ExecutionResultSteps(TestExecutionResult testExecutionResult, HooksDriver hooksDriver)
        {
            this.testExecutionResult = testExecutionResult;
            this.hooksDriver = hooksDriver;
        }

        public TestRunSummary ConvertToSummary(Table table)
        {
            return table.CreateInstance<TestRunSummary>();
        }

        [Then(@"all tests should pass")]
        [Then(@"the scenario should pass")]
        public void ThenAllTestsShouldPass()
        {
            testExecutionResult.LastExecutionSummary.Should().NotBeNull();
            testExecutionResult.LastExecutionSummary.Succeeded.Should().Be(testExecutionResult.LastExecutionSummary.Total);
        }

        [Then(@"the execution summary should contain")]
        public void ThenTheExecutionSummaryShouldContain(Table expectedSummary)
        {
            testExecutionResult.LastExecutionSummary.Should().NotBeNull();
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
            testExecutionResult.ExecutionLog.Should().NotBeNull("no execution log generated");

            var regex = new Regex(@"-> done: \S+\." + methodName);
            if (times > 0)
                regex.Match(testExecutionResult.ExecutionLog).Success.Should().BeTrue("method " + methodName + " was not executed.");

            if (times != int.MaxValue)
                regex.Matches(testExecutionResult.ExecutionLog).Count.Should().Be(times);
        }

        [Then(@"the hook '(.*)' is executed (\D.*)")]
        [Then(@"the hook '(.*)' is executed (\d+) times")]
        public void ThenTheHookIsExecuted(string methodName, int times)
        {
            var hookLog = hooksDriver.HookLog;
            hookLog.Should().NotBeNullOrEmpty("no execution log generated");

            var regex = new Regex(@"-> hook: " + methodName);
            if (times > 0)
                regex.Match(hookLog).Success.Should().BeTrue("method " + methodName + " was not executed.");

            if (times != int.MaxValue)
                regex.Matches(hookLog).Count.Should().Be(times);
        }

        [Then(@"the hooks are executed in the order")]
        public void ThenTheHooksAreExecutedInTheOrder(Table table)
        {
            var hookLog = hooksDriver.HookLog;
            hookLog.Should().NotBeNullOrEmpty("no execution log generated");
            int lastPosition = -1;
            foreach (var row in table.Rows)
            {
                int currentPosition = hookLog.IndexOf(@"-> hook: " + row[0]);
                currentPosition.Should().BeGreaterThan(lastPosition);
                lastPosition = currentPosition;
            }
        }

        [Then(@"the execution log should contain text '(.*)'")]
        public void ThenTheExecutionLogShouldContainText(string text)
        {
            testExecutionResult.ExecutionLog.Should().NotBeNull("no execution log generated");
            testExecutionResult.ExecutionLog.Should().Contain(text);
        }

        [Given(@"the log file '(.*)' is empty")]
        public void GivenTheLogFileIsEmpty(string logFilePath)
        {
            File.WriteAllText(GetPath(logFilePath), "");
        }

        private string GetPath(string logFilePath)
        {
            string filePath = Path.Combine(Path.GetTempPath(), logFilePath);
            return filePath;
        }

        [Then(@"the log file '(.*)' should contain text '(.*)'")]
        public void ThenTheLogFileShouldContainText(string logFilePath, string text)
        {
            var logContent = File.ReadAllText(GetPath(logFilePath));
            logContent.Should().Contain(text);
        }

        [Then(@"the log file '(.*)' should contain the text '(.*)' (\d+) times")]
        public void ThenTheLogFileShouldContainTHETextTimes(string logFilePath, string text, int times)
        {
            var logConent = File.ReadAllText(GetPath(logFilePath));
            logConent.Should().NotBeNullOrEmpty("no trace log is generated");

            var regex = new Regex(text, RegexOptions.Multiline);
            if (times > 0)
                regex.Match(logConent).Success.Should().BeTrue(text + " was not found in the logs");

            if (times != int.MaxValue) 
                 regex.Matches(logConent).Count.Should().Be(times);
        }
    }
}