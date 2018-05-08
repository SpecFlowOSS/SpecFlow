using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using SpecFlow.TestProjectGenerator.NewApi.Driver;
using SpecFlow.TestProjectGenerator.NewApi._5_TestRun;
using TechTalk.SpecFlow.Specs.Drivers;
using TechTalk.SpecFlow.Assist;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class ExecutionResultSteps
    {
        private readonly HooksDriver _hooksDriver;
        private readonly VSTestExecutionDriver _vsTestExecutionDriver;

        public ExecutionResultSteps(HooksDriver hooksDriver, VSTestExecutionDriver vsTestExecutionDriver)
        {
            _hooksDriver = hooksDriver;
            _vsTestExecutionDriver = vsTestExecutionDriver;
        }

        public TestRunSummary ConvertToSummary(Table table)
        {
            return table.CreateInstance<TestRunSummary>();
        }

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
            // TODO: do not search in output but in execution log
            _vsTestExecutionDriver.CheckOutputContainsText(text);
        }

        [Given(@"the log file '(.*)' is empty")]
        public void GivenTheLogFileIsEmpty(string logFilePath)
        {
            File.WriteAllText(GetPath(logFilePath), "");
        }

        [Then(@"the log file '(.*)' should contain text '(.*)'")]
        public void ThenTheLogFileShouldContainText(string logFilePath, string text)
        {
            var logContent = File.ReadAllText(GetPath(logFilePath));
            logContent.Should().Contain(text);
        }

        [Then(@"the log file '(.*)' should contain the text '(.*)' (\d+) times")]
        public void ThenTheLogFileShouldContainTheTextTimes(string logFilePath, string text, int times)
        {
            var logConent = File.ReadAllText(GetPath(logFilePath));
            logConent.Should().NotBeNullOrEmpty("no trace log is generated");

            var regex = new Regex(text, RegexOptions.Multiline);
            if (times > 0)
                regex.Match(logConent).Success.Should().BeTrue(text + " was not found in the logs");

            if (times != int.MaxValue) 
                 regex.Matches(logConent).Count.Should().Be(times);
        }

        private string GetPath(string logFilePath)
        {
            string filePath = Path.Combine(Path.GetTempPath(), logFilePath);
            return filePath;
        }
    }
}