using System.Diagnostics;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TechTalk.SpecFlow.xUnit.SpecFlowPlugin.Extensions.AssemblyFixture
{
    public class XunitTestFrameworkWithAssemblyFixture : XunitTestFramework
    {
        public XunitTestFrameworkWithAssemblyFixture(IMessageSink messageSink)
            : base(messageSink)
        { }

        protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
        {
            Debugger.Launch();
            return new XunitTestFrameworkExecutorWithAssemblyFixture(assemblyName, SourceInformationProvider, DiagnosticMessageSink);
        }
    }
}