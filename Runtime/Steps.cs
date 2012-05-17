namespace TechTalk.SpecFlow
{
    public abstract class Steps
    {
        protected readonly ITestRunner testRunner;

        protected Steps(ITestRunner testRunner)
        {
            this.testRunner = testRunner;
        }

        protected Steps() : this(ScenarioContext.Current.TestRunner)  // This will be AsyncTestRunner for asynchronous tests
        {
            testRunner = ScenarioContext.Current.TestRunner;
        }

        #region Given
        public void Given(string step)
        {
            Given(step, null, null);
        }

        public void Given(string step, Table tableArg)
        {
            Given(step, null, tableArg);
        }

        public void Given(string step, string multilineTextArg)
        {
            Given(step, multilineTextArg, null);
        }

        public void Given(string step, string multilineTextArg, Table tableArg)
        {
            testRunner.Given(step, multilineTextArg, tableArg, null);
        }
        #endregion

        #region When
        public void When(string step)
        {
            When(step, null, null);
        }

        public void When(string step, Table tableArg)
        {
            When(step, null, tableArg);
        }

        public void When(string step, string multilineTextArg)
        {
            When(step, multilineTextArg, null);
        }

        public void When(string step, string multilineTextArg, Table tableArg)
        {
            testRunner.When(step, multilineTextArg, tableArg, null);
        }
        #endregion

        #region Then
        public void Then(string step)
        {
            Then(step, null, null);
        }

        public void Then(string step, Table tableArg)
        {
            Then(step, null, tableArg);
        }

        public void Then(string step, string multilineTextArg)
        {
            Then(step, multilineTextArg, null);
        }

        public void Then(string step, string multilineTextArg, Table tableArg)
        {
            testRunner.Then(step, multilineTextArg, tableArg, null);
        }
        #endregion

        #region But
        public void But(string step)
        {
            But(step, null, null);
        }

        public void But(string step, Table tableArg)
        {
            But(step, null, tableArg);
        }

        public void But(string step, string multilineTextArg)
        {
            But(step, multilineTextArg, null);
        }

        public void But(string step, string multilineTextArg, Table tableArg)
        {
            testRunner.But(step, multilineTextArg, tableArg, null);
        }
        #endregion

        #region And
        public void And(string step)
        {
            And(step, null, null);
        }

        public void And(string step, Table tableArg)
        {
            And(step, null, tableArg);
        }

        public void And(string step, string multilineTextArg)
        {
            And(step, multilineTextArg, null);
        }

        public void And(string step, string multilineTextArg, Table tableArg)
        {
            testRunner.And(step, multilineTextArg, tableArg, null);
        }
        #endregion
    }
}
