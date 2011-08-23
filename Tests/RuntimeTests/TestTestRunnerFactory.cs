using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoDi;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.RuntimeTests
{
    public static class TestTestRunnerFactory
    {
        static internal TestRunner CreateTestRunner(out IObjectContainer container, Action<IObjectContainer> registerMocks = null)
        {
            container = TestRunContainerBuilder.CreateContainer();

            if (registerMocks != null)
                registerMocks(container);

            return (TestRunner)container.Resolve<ITestRunner>();
        }

        static internal TestRunner CreateTestRunner(Action<IObjectContainer> registerMocks = null)
        {
            IObjectContainer container;
            return CreateTestRunner(out container, registerMocks);
        }
    }
}
