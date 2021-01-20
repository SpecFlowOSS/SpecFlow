using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.TestProjectGenerator;
using TechTalk.SpecFlow.TestProjectGenerator.Driver;
using TechTalk.SpecFlow.TestProjectGenerator.Helpers;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class ExecutionResultSteps
    {
        private readonly HooksDriver _hooksDriver;
        private readonly VSTestExecutionDriver _vsTestExecutionDriver;
        private readonly TestProjectFolders _testProjectFolders;
        private readonly TestRunLogDriver _testRunLogDriver;

        public ExecutionResultSteps(HooksDriver hooksDriver, VSTestExecutionDriver vsTestExecutionDriver, TestProjectFolders testProjectFolders, TestRunLogDriver testRunLogDriver)
        {
            _hooksDriver = hooksDriver;
            _vsTestExecutionDriver = vsTestExecutionDriver;
            _testProjectFolders = testProjectFolders;
            _testRunLogDriver = testRunLogDriver;
        }

        [Then(@"the tests were executed successfully")]
        [Then(@"all tests should pass")]
        [Then(@"the scenario should pass")]
        public void ThenAllTestsShouldPass()
        {
            _vsTestExecutionDriver.LastTestExecutionResult.Should().NotBeNull();
            _vsTestExecutionDriver.LastTestExecutionResult.Succeeded.Should().Be(_vsTestExecutionDriver.LastTestExecutionResult.Total);
        }

        [Then(@"the execution summary should contain")]
        public void ThenTheExecutionSummaryShouldContain(Table expectedTestExecutionResult)
        {
            _vsTestExecutionDriver.LastTestExecutionResult.Should().NotBeNull();
            expectedTestExecutionResult.CompareToInstance(_vsTestExecutionDriver.LastTestExecutionResult);
        }

        [Then(@"the binding method '(.*)' is executed")]
        public void ThenTheBindingMethodIsExecuted(string methodName)
        {
            ThenTheBindingMethodIsExecuted(methodName, 1);
        }

        [Then(@"the binding method '(.*)' is executed (.*)")]
        public void ThenTheBindingMethodIsExecuted(string methodName, int times)
        {
            _vsTestExecutionDriver.CheckIsBindingMethodExecuted(methodName, times);
        }

        [Then(@"the hook '(.*)' is executed (\D.*)")]
        [Then(@"the hook '(.*)' is executed (\d+) times")]
        public void ThenTheHookIsExecuted(string methodName, int times)
        {
            _hooksDriver.CheckIsHookExecuted(methodName, times);
        }

        [Then(@"the hooks are executed in the order")]
        public void ThenTheHooksAreExecutedInTheOrder(Table table)
        {
            _hooksDriver.CheckIsHookExecutedInOrder(table.Rows.Select(r => r[0]));
        }

        [Then(@"the execution log should contain text '(.*)'")]
        public void ThenTheExecutionLogShouldContainText(string text)
        {
            _vsTestExecutionDriver.CheckAnyOutputContainsText(text);
        }

        [Then(@"the execution log should contain text")]
        public void ThenTheExecutionLogShouldContainMultilineText(string multilineText)
        {
            _vsTestExecutionDriver.CheckAnyOutputContainsText(multilineText);
        }

        [Then(@"the output should contain text '(.*)'")]
        public void ThenTheOutputShouldContainText(string text)
        {
            _vsTestExecutionDriver.CheckOutputContainsText(text);
        }
        
        [Then(@"the log file '(.*)' should contain text '(.*)'")]
        public void ThenTheLogFileShouldContainText(string logFilePath, string text)
        {
            _testRunLogDriver.CheckLogContainsText(text, logFilePath);
        }

        [Then(@"the log file '(.*)' should contain the text '(.*)' (\d+) times")]
        public void ThenTheLogFileShouldContainTheTextTimes(string logFilePath, string regexString, int times)
        {
            _testRunLogDriver.CheckLogMatchesRegexTimes(regexString, times, logFilePath);
        }

        private string GetPath(string logFilePath)
        {
            string filePath = Path.Combine(_testProjectFolders.ProjectFolder, logFilePath);
            return filePath;
        }

        [Then(@"every scenario has it's individual context id")]
        public void ThenEveryScenarioHasItSIndividualContextId()
        {
            var lastTestExecutionResult = _vsTestExecutionDriver.LastTestExecutionResult;

            foreach (var testResult in lastTestExecutionResult.TestResults)
            {
                var contextIdLines = testResult.StdOut.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries).Where(s => s.StartsWith("Context ID"));

                var distinctContextIdLines = contextIdLines.Distinct();

                distinctContextIdLines.Count().Should().Be(1);
            }
        }
    }
}
