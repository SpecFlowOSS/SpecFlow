namespace TechTalk.SpecFlow.CucumberMessages
{
    public class NonStrictTestRunResultSuccessCalculator : ITestRunResultSuccessCalculator
    {
        public bool IsSuccess(TestRunResult testRunResult)
        {
            return testRunResult.Failed == 0
                   && testRunResult.Ambiguous == 0;
        }
    }
}
