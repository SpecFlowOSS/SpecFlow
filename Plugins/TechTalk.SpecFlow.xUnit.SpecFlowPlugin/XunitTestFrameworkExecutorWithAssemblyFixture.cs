// Contains code from https://github.com/xunit/samples.xunit
// originally published under Apache 2.0 license
// For more information see aforementioned repository
using System.Collections.Generic;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TechTalk.SpecFlow.xUnit.SpecFlowPlugin
{
    public class XunitTestFrameworkExecutorWithAssemblyFixture : XunitTestFrameworkExecutor
    {
        public XunitTestFrameworkExecutorWithAssemblyFixture(AssemblyName assemblyName, ISourceInformationProvider sourceInformationProvider, IMessageSink diagnosticMessageSink)
            : base(assemblyName, sourceInformationProvider, diagnosticMessageSink)
        {
        }

        protected override async void RunTestCases(IEnumerable<IXunitTestCase> testCases, IMessageSink executionMessageSink, ITestFrameworkExecutionOptions executionOptions)
        {
            using (var assemblyRunner = new XunitTestAssemblyRunnerWithAssemblyFixture(TestAssembly, testCases, DiagnosticMessageSink, executionMessageSink, executionOptions))
            {
                await assemblyRunner.RunAsync();
            }
        }
    }
}
