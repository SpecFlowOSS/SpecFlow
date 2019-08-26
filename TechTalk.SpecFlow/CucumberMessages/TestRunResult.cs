namespace TechTalk.SpecFlow.CucumberMessages
{
    public class TestRunResult
    {
        public TestRunResult(int total, int passed, int failed, int skipped, int ambiguous, int undefined)
        {
            Total = total;
            Passed = passed;
            Failed = failed;
            Skipped = skipped;
            Ambiguous = ambiguous;
            Undefined = undefined;
        }

        public int Total { get; }

        public int Passed { get; }

        public int Failed { get; }

        public int Skipped { get; }

        public int Ambiguous { get; }

        public int Undefined { get; }
    }
}
