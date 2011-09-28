using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TechTalk.SpecFlow.Infrastructure
{
    public interface ITestRunnerFactory
    {
        ITestRunner Create(Assembly testAssembly);
    }
}
