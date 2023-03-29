using System;
using BoDi;

namespace TechTalk.SpecFlow
{
    public interface ITestThreadContext : ISpecFlowContext
    {
        IObjectContainer TestThreadContainer { get; }
    }

    public class TestThreadContext : SpecFlowContext, ITestThreadContext
    {
        public event Action<TestThreadContext> Disposing;
        public IObjectContainer TestThreadContainer { get; }

        public TestThreadContext(IObjectContainer testThreadContainer)
        {
            TestThreadContainer = testThreadContainer;
        }

        protected override void Dispose()
        {
            Disposing?.Invoke(this);
            base.Dispose();
        }
    }
}
