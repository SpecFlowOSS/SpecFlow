namespace TechTalk.SpecFlow
{
    public abstract class Steps
    {
        private static ITestRunner _testRunner;

        protected Steps()
        {
            _testRunner = TestRunnerManager.GetTestRunner();
        }

        public void Given(string step)
        {
            _testRunner.Given(step);
        }   
        
        public void When(string step)
        {
            _testRunner.When(step);
        }   

        public void And(string step)
        {
            _testRunner.And(step);
        }
    }
}
