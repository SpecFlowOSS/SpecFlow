using System.Linq;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.TestProjectGenerator.CucumberMessages;
using TechTalk.SpecFlow.TestProjectGenerator.CucumberMessages.RowObjects;

namespace TechTalk.SpecFlow.Specs.Drivers.CucumberMessages
{
    public class MessageValidationDriver
    {
        private readonly TestCaseFinishedDriver _testCaseFinishedDriver;
        private readonly TestRunStartedDriver _testRunStartedDriver;
        private readonly TestCaseStartedDriver _testCaseStartedDriver;

        public MessageValidationDriver(TestCaseFinishedDriver testCaseFinishedDriver, TestRunStartedDriver testRunStartedDriver, TestCaseStartedDriver testCaseStartedDriver)
        {
            _testCaseFinishedDriver = testCaseFinishedDriver;
            _testRunStartedDriver = testRunStartedDriver;
            _testCaseStartedDriver = testCaseStartedDriver;
        }

        public void TestCaseFinishedMessageShouldHaveBeenSent(Table values)
        {
            var testCaseFinishedRow = values.CreateInstance<TestCaseFinishedRow>();
            _testCaseFinishedDriver.TestCaseFinishedMessageShouldHaveBeenSent(testCaseFinishedRow);
        }

        public void TestCaseFinishedMessageShouldHaveBeenSentWithTestResult(Table values)
        {
            var testResultRow = values.CreateInstance<TestResultRow>();
            _testCaseFinishedDriver.TestCaseFinishedMessageShouldHaveBeenSentWithTestResult(testResultRow);
        }

        public void TestRunStartedMessageShouldHaveBeenSent(Table values)
        {
            var testRunStartedRow = values.CreateInstance<TestRunStartedRow>();
            _testRunStartedDriver.TestRunStartedMessageShouldHaveBeenSent(testRunStartedRow);
        }

        public void TestCaseStartedMessageShouldHaveBeenSent(Table values)
        {
            var testCaseStartedRow = values.CreateInstance<TestCaseStartedRow>();
            _testCaseStartedDriver.TestCaseStartedMessageShouldHaveBeenSent(testCaseStartedRow);
        }

        public void TestCaseStartedMessageShouldHaveBeenSentWithPlatformInformation(Table values)
        {
            var platformRow = values.CreateInstance<PlatformRow>();
            _testCaseStartedDriver.TestCaseStartedMessageShouldHaveBeenSentWithPlatformInformation(platformRow);
        }

        public void TestCaseStartedMessageShouldHaveBeenSentWithPlatformInformationAttributes(Table attributes)
        {
            var expectedAttributes = attributes.Rows.Select(r => r["Attribute"]);
            _testCaseStartedDriver.TestCaseStartedMessageShouldHaveBeenSentWithPlatformInformationAttributes(expectedAttributes);
        }

        public void TestRunFinishedMessageShouldHaveBeenSent(Table values)
        {
            var testRunFinishedRow = values.CreateInstance<TestRunFinishedRow>();
            _testRunStartedDriver.TestRunFinishedMessageShouldHaveBeenSent(testRunFinishedRow);

        }
    }
}
