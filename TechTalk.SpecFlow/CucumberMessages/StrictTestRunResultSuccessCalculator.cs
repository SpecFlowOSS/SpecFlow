namespace TechTalk.SpecFlow.CucumberMessages
{
    public class StrictTestRunResultSuccessCalculator : ITestRunResultSuccessCalculator
    {
        public bool IsSuccess(TestRunResult testRunResult)
        {
            return testRunResult.Failed == 0
                   && testRunResult.Skipped == 0
                   && testRunResult.Ambiguous == 0
                   && testRunResult.Undefined == 0;
        }
    }
}
