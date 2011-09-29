using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        [Then(@"the execution summary should contain")]
        public void ThenTheExecutionSummaryShouldContain(Table expectedSummary)
        {
            testExecutionResult.LastExecutionSummary.ShouldNotBeNull();
            expectedSummary.CompareToInstance(testExecutionResult.LastExecutionSummary);
        }
    }
}
