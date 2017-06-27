using BoDi;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow
{
    public abstract class Steps : IContainerDependentObject
    {
        private IObjectContainer objectContainer;

        void IContainerDependentObject.SetObjectContainer(IObjectContainer container)
        {
            if (objectContainer != null)
                throw new SpecFlowException("Container of the steps class has already initialized!");

            objectContainer = container;
        }

        protected ITestRunner TestRunner
        {
            get
            {
                AssertInitialized();
                return objectContainer.Resolve<ITestRunner>();
            }
        }

        public ScenarioContext ScenarioContext
        {
            get
            {
                AssertInitialized();
                return objectContainer.Resolve<ScenarioContext>();
            }
        }

        public FeatureContext FeatureContext
        {
            get
            {
                AssertInitialized();
                return objectContainer.Resolve<FeatureContext>();
            }
        }

        public TestThreadContext TestThreadContext
        {
            get
            {
                AssertInitialized();
                return objectContainer.Resolve<TestThreadContext>();
            }
        }

        public ScenarioStepContext StepContext
        {
            get
            {
                AssertInitialized();
                var contextManager = objectContainer.Resolve<IContextManager>();
                return contextManager.StepContext;
            }
        }

        protected void AssertInitialized()
        {
            if (objectContainer == null)
                throw new SpecFlowException("Container of the steps class has not been initialized!");
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
            TestRunner.Given(step, multilineTextArg, tableArg, null);
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
            TestRunner.When(step, multilineTextArg, tableArg, null);
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
            TestRunner.Then(step, multilineTextArg, tableArg, null);
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
            TestRunner.But(step, multilineTextArg, tableArg, null);
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
            TestRunner.And(step, multilineTextArg, tableArg, null);
        }
        #endregion
    }
}
