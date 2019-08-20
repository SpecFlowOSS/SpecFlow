using System.Threading.Tasks;
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
        public async Task GivenAsync(string step)
        {
            await GivenAsync(step, null, null);
        }

        public async Task GivenAsync(string step, Table tableArg)
        {
            await GivenAsync(step, null, tableArg);
        }

        public async Task GivenAsync(string step, string multilineTextArg)
        {
            await GivenAsync(step, multilineTextArg, null);
        }

        public async Task GivenAsync(string step, string multilineTextArg, Table tableArg)
        {
            await TestRunner.GivenAsync(step, multilineTextArg, tableArg, null);
        }
        #endregion

        #region When
        public async Task WhenAsync(string step)
        {
            await WhenAsync(step, null, null);
        }

        public async Task WhenAsync(string step, Table tableArg)
        {
            await WhenAsync(step, null, tableArg);
        }

        public async Task WhenAsync(string step, string multilineTextArg)
        {
            await WhenAsync(step, multilineTextArg, null);
        }

        public async Task WhenAsync(string step, string multilineTextArg, Table tableArg)
        {
            await TestRunner.WhenAsync(step, multilineTextArg, tableArg, null);
        }
        #endregion

        #region Then
        public async Task ThenAsync(string step)
        {
            await ThenAsync(step, null, null);
        }

        public async Task ThenAsync(string step, Table tableArg)
        {
            await ThenAsync(step, null, tableArg);
        }

        public async Task ThenAsync(string step, string multilineTextArg)
        {
            await ThenAsync(step, multilineTextArg, null);
        }

        public async Task ThenAsync(string step, string multilineTextArg, Table tableArg)
        {
            await TestRunner.ThenAsync(step, multilineTextArg, tableArg, null);
        }
        #endregion

        #region But
        public async Task ButAsync(string step)
        {
            await ButAsync(step, null, null);
        }

        public async Task ButAsync(string step, Table tableArg)
        {
            await ButAsync(step, null, tableArg);
        }

        public async Task ButAsync(string step, string multilineTextArg)
        {
            await ButAsync(step, multilineTextArg, null);
        }

        public async Task ButAsync(string step, string multilineTextArg, Table tableArg)
        {
            await TestRunner.ButAsync(step, multilineTextArg, tableArg, null);
        }
        #endregion

        #region And
        public async Task AndAsync(string step)
        {
            await AndAsync(step, null, null);
        }

        public async Task AndAsync(string step, Table tableArg)
        {
            await AndAsync(step, null, tableArg);
        }

        public async Task AndAsync(string step, string multilineTextArg)
        {
            await AndAsync(step, multilineTextArg, null);
        }

        public async Task AndAsync(string step, string multilineTextArg, Table tableArg)
        {
            await TestRunner.AndAsync(step, multilineTextArg, tableArg, null);
        }
        #endregion
    }
}
