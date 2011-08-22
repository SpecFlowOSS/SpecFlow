using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniDi;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.RuntimeTests
{
    public static class TestTestRunnerFactory
    {
        static internal TestRunner CreateTestRunner(Action<IObjectContainer> registerMocks = null)
        {
            var container = TestRunContainerBuilder.CreateContainer();

            if (registerMocks != null)
                registerMocks(container);

            return (TestRunner)container.Resolve<ITestRunner>();
        }
    }
}
