using System;
using System.ComponentModel;
using System.Threading.Tasks;
using BoDi;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow
{
    public abstract class Steps : IContainerDependentObject
    {
        private const string GivenObsoleteMessage = $"{nameof(Steps)}.{nameof(Given)} and {nameof(Steps)}.{nameof(GivenAsync)} are obsolete and will be removed with SpecFlow 4.0. Details: https://github.com/techtalk/SpecFlow/issues/1733";
        private const string WhenObsoleteMessage = $"{nameof(Steps)}.{nameof(When)} and {nameof(Steps)}.{nameof(WhenAsync)} are obsolete and will be removed with SpecFlow 4.0. Details: https://github.com/techtalk/SpecFlow/issues/1733";
        private const string ThenObsoleteMessage = $"{nameof(Steps)}.{nameof(Then)} and {nameof(Steps)}.{nameof(ThenAsync)} are obsolete and will be removed with SpecFlow 4.0. Details: https://github.com/techtalk/SpecFlow/issues/1733";
        private const string ButObsoleteMessage = $"{nameof(Steps)}.{nameof(And)} and {nameof(Steps)}.{nameof(AndAsync)} are obsolete and will be removed with SpecFlow 4.0. Details: https://github.com/techtalk/SpecFlow/issues/1733";
        private const string AndObsoleteMessage = $"{nameof(Steps)}.{nameof(But)} and {nameof(Steps)}.{nameof(ButAsync)} are obsolete and will be removed with SpecFlow 4.0. Details: https://github.com/techtalk/SpecFlow/issues/1733";
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

        protected ISyncTestRunner SyncTestRunner
        {
            get
            {
                AssertInitialized();
                return objectContainer.Resolve<ISyncTestRunner>();
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

        #region GivenAsync
        [Obsolete(GivenObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async Task GivenAsync(string step)
        {
            await GivenAsync(step, null, null);
        }

        [Obsolete(GivenObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async Task GivenAsync(string step, Table tableArg)
        {
            await GivenAsync(step, null, tableArg);
        }

        [Obsolete(GivenObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async Task GivenAsync(string step, string multilineTextArg)
        {
            await GivenAsync(step, multilineTextArg, null);
        }

        [Obsolete(GivenObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async Task GivenAsync(string step, string multilineTextArg, Table tableArg)
        {
            await TestRunner.GivenAsync(step, multilineTextArg, tableArg);
        }
        #endregion

        #region WhenAsync
        [Obsolete(WhenObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async Task WhenAsync(string step)
        {
            await WhenAsync(step, null, null);
        }

        [Obsolete(WhenObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async Task WhenAsync(string step, Table tableArg)
        {
            await WhenAsync(step, null, tableArg);
        }

        [Obsolete(WhenObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async Task WhenAsync(string step, string multilineTextArg)
        {
            await WhenAsync(step, multilineTextArg, null);
        }

        [Obsolete(WhenObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async Task WhenAsync(string step, string multilineTextArg, Table tableArg)
        {
            await TestRunner.WhenAsync(step, multilineTextArg, tableArg);
        }
        #endregion

        #region ThenAsync
        [Obsolete(ThenObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async Task ThenAsync(string step)
        {
            await ThenAsync(step, null, null);
        }

        [Obsolete(ThenObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async Task ThenAsync(string step, Table tableArg)
        {
            await ThenAsync(step, null, tableArg);
        }

        [Obsolete(ThenObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async Task ThenAsync(string step, string multilineTextArg)
        {
            await ThenAsync(step, multilineTextArg, null);
        }

        [Obsolete(ThenObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async Task ThenAsync(string step, string multilineTextArg, Table tableArg)
        {
            await TestRunner.ThenAsync(step, multilineTextArg, tableArg);
        }
        #endregion

        #region ButAsync
        [Obsolete(ButObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async Task ButAsync(string step)
        {
            await ButAsync(step, null, null);
        }

        [Obsolete(ButObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async Task ButAsync(string step, Table tableArg)
        {
            await ButAsync(step, null, tableArg);
        }

        [Obsolete(ButObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async Task ButAsync(string step, string multilineTextArg)
        {
            await ButAsync(step, multilineTextArg, null);
        }

        [Obsolete(ButObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async Task ButAsync(string step, string multilineTextArg, Table tableArg)
        {
            await TestRunner.ButAsync(step, multilineTextArg, tableArg);
        }
        #endregion

        #region AndAsync
        [Obsolete(AndObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async Task AndAsync(string step)
        {
            await AndAsync(step, null, null);
        }

        [Obsolete(AndObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async Task AndAsync(string step, Table tableArg)
        {
            await AndAsync(step, null, tableArg);
        }

        [Obsolete(AndObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async Task AndAsync(string step, string multilineTextArg)
        {
            await AndAsync(step, multilineTextArg, null);
        }

        [Obsolete(AndObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async Task AndAsync(string step, string multilineTextArg, Table tableArg)
        {
            await TestRunner.AndAsync(step, multilineTextArg, tableArg);
        }
        #endregion

        #region Given
        [Obsolete(GivenObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Given(string step)
        {
            Given(step, null, null);
        }

        [Obsolete(GivenObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Given(string step, Table tableArg)
        {
            Given(step, null, tableArg);
        }

        [Obsolete(GivenObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Given(string step, string multilineTextArg)
        {
            Given(step, multilineTextArg, null);
        }

        [Obsolete(GivenObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Given(string step, string multilineTextArg, Table tableArg)
        {
            SyncTestRunner.Given(step, multilineTextArg, tableArg);
        }
        #endregion

        #region When
        [Obsolete(WhenObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void When(string step)
        {
            When(step, null, null);
        }

        [Obsolete(WhenObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void When(string step, Table tableArg)
        {
            When(step, null, tableArg);
        }

        [Obsolete(WhenObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void When(string step, string multilineTextArg)
        {
            When(step, multilineTextArg, null);
        }

        [Obsolete(WhenObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void When(string step, string multilineTextArg, Table tableArg)
        {
            SyncTestRunner.When(step, multilineTextArg, tableArg);
        }
        #endregion

        #region Then
        [Obsolete(ThenObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Then(string step)
        {
            Then(step, null, null);
        }

        [Obsolete(ThenObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Then(string step, Table tableArg)
        {
            Then(step, null, tableArg);
        }

        [Obsolete(ThenObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Then(string step, string multilineTextArg)
        {
            Then(step, multilineTextArg, null);
        }

        [Obsolete(ThenObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Then(string step, string multilineTextArg, Table tableArg)
        {
            SyncTestRunner.Then(step, multilineTextArg, tableArg);
        }
        #endregion

        #region But
        [Obsolete(ButObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void But(string step)
        {
            But(step, null, null);
        }

        [Obsolete(ButObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void But(string step, Table tableArg)
        {
            But(step, null, tableArg);
        }

        [Obsolete(ButObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void But(string step, string multilineTextArg)
        {
            But(step, multilineTextArg, null);
        }

        [Obsolete(ButObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void But(string step, string multilineTextArg, Table tableArg)
        {
            SyncTestRunner.But(step, multilineTextArg, tableArg);
        }
        #endregion

        #region And
        [Obsolete(AndObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void And(string step)
        {
            And(step, null, null);
        }

        [Obsolete(AndObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void And(string step, Table tableArg)
        {
            And(step, null, tableArg);
        }

        [Obsolete(AndObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void And(string step, string multilineTextArg)
        {
            And(step, multilineTextArg, null);
        }

        [Obsolete(AndObsoleteMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void And(string step, string multilineTextArg, Table tableArg)
        {
            SyncTestRunner.And(step, multilineTextArg, tableArg);
        }
        #endregion
    }
}
