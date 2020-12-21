using System;
using System.ComponentModel;
using System.Threading.Tasks;
using BoDi;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow
{
    public abstract class Steps : IContainerDependentObject
    {
        private const string GivenObsoleteMessage = nameof(Steps) + "." + nameof(GivenAsync) + " is obsolete and will be removed with SpecFlow 4.0. Details: https://github.com/techtalk/SpecFlow/issues/1733";
        private const string WhenObsoleteMessage = nameof(Steps) + "." + nameof(WhenAsync) + " is obsolete and will be removed with SpecFlow 4.0. Details: https://github.com/techtalk/SpecFlow/issues/1733";
        private const string ThenObsoleteMessage = nameof(Steps) + "." + nameof(ThenAsync) + " is obsolete and will be removed with SpecFlow 4.0. Details: https://github.com/techtalk/SpecFlow/issues/1733";
        private const string ButObsoleteMessage = nameof(Steps) + "." + nameof(ButAsync) + " is obsolete and will be removed with SpecFlow 4.0. Details: https://github.com/techtalk/SpecFlow/issues/1733";
        private const string AndObsoleteMessage = nameof(Steps) + "." + nameof(AndAsync) + " is obsolete and will be removed with SpecFlow 4.0. Details: https://github.com/techtalk/SpecFlow/issues/1733";
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
            await TestRunner.GivenAsync(step, multilineTextArg, tableArg, null);
        }
        #endregion

        #region When
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
            await TestRunner.WhenAsync(step, multilineTextArg, tableArg, null);
        }
        #endregion

        #region Then
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
            await TestRunner.ThenAsync(step, multilineTextArg, tableArg, null);
        }
        #endregion

        #region But
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
            await TestRunner.ButAsync(step, multilineTextArg, tableArg, null);
        }
        #endregion

        #region And
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
            await TestRunner.AndAsync(step, multilineTextArg, tableArg, null);
        }
        #endregion
    }
}
