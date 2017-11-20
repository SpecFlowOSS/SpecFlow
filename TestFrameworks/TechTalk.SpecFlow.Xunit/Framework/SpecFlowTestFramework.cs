﻿using Xunit.Abstractions;
using Xunit.Sdk;

namespace TechTalk.SpecFlow.Xunit.Framework
{
    public class SpecFlowTestFramework : XunitTestFramework
    {
        public SpecFlowTestFramework(IMessageSink diagnosticMessageSink) : base(diagnosticMessageSink)
        {
        }

        protected override ITestFrameworkDiscoverer CreateDiscoverer(IAssemblyInfo assemblyInfo)
        {
            return new SpecFlowTestDiscoverer(assemblyInfo, SourceInformationProvider, DiagnosticMessageSink);
        }

        //protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
        //{
        //    return new SpecFlowTestFrameworkExecutor(assemblyName, SourceInformationProvider, DiagnosticMessageSink);
        //}
    }
}