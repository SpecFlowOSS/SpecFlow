using System;
using System.ComponentModel;
using BoDi;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow
{
    public abstract class Steps : IContainerDependentObject
    {
        private const string GivenObsoleteMessage = nameof(Steps) + "." + nameof(Given) + " is obsolete and will be removed with SpecFlow 4.0. Details: https://github.com/techtalk/SpecFlow/issues/1733";
        private const string WhenObsoleteMessage = nameof(Steps) + "." + nameof(When) + " is obsolete and will be removed with SpecFlow 4.0. Details: https://github.com/techtalk/SpecFlow/issues/1733";
        private const string ThenObsoleteMessage = nameof(Steps) + "." + nameof(Then) + " is obsolete and will be removed with SpecFlow 4.0. Details: https://github.com/techtalk/SpecFlow/issues/1733";
        private const string ButObsoleteMessage = nameof(Steps) + "." + nameof(But) + " is obsolete and will be removed with SpecFlow 4.0. Details: https://github.com/techtalk/SpecFlow/issues/1733";
        private const string AndObsoleteMessage = nameof(Steps) + "." + nameof(And) + " is obsolete and will be removed with SpecFlow 4.0. Details: https://github.com/techtalk/SpecFlow/issues/1733";
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
            TestRunner.Given(step, multilineTextArg, tableArg, null);
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
            TestRunner.When(step, multilineTextArg, tableArg, null);
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
            TestRunner.Then(step, multilineTextArg, tableArg, null);
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
            TestRunner.But(step, multilineTextArg, tableArg, null);
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
            TestRunner.And(step, multilineTextArg, tableArg, null);
        }
        #endregion
    }
}
