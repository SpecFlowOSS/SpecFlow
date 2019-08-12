﻿// Contains code from https://github.com/xunit/samples.xunit
// originally published under Apache 2.0 license
// For more information see aforementioned repository
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TechTalk.SpecFlow.xUnit.SpecFlowPlugin
{
    public class XunitTestAssemblyRunnerWithAssemblyFixture : XunitTestAssemblyRunner
    {
        private readonly Dictionary<Type, object> _assemblyFixtureMappings = new Dictionary<Type, object>();

        public XunitTestAssemblyRunnerWithAssemblyFixture(
            ITestAssembly testAssembly,
            IEnumerable<IXunitTestCase> testCases,
            IMessageSink diagnosticMessageSink,
            IMessageSink executionMessageSink,
            ITestFrameworkExecutionOptions executionOptions)
            : base(
                testAssembly,
                testCases,
                diagnosticMessageSink,
                executionMessageSink,
                executionOptions)
        {
        }

        protected override async Task AfterTestAssemblyStartingAsync()
        {
            // Let everything initialize
            await base.AfterTestAssemblyStartingAsync();

            // Go find all the AssemblyFixtureAttributes adorned on the test assembly
            Aggregator.Run(() =>
            {
                var reflectionAssemblyInfo = (IReflectionAssemblyInfo)TestAssembly.Assembly;
                var fixturesAttrs = reflectionAssemblyInfo.Assembly
                                                          .GetCustomAttributes(typeof(AssemblyFixtureAttribute), false)
                                                          .Cast<AssemblyFixtureAttribute>()
                                                          .ToList();

                // Instantiate all the fixtures
                foreach (var fixtureAttr in fixturesAttrs)
                {
                    _assemblyFixtureMappings[fixtureAttr.FixtureType] = Activator.CreateInstance(fixtureAttr.FixtureType);
                }
            });
        }

        protected override Task BeforeTestAssemblyFinishedAsync()
        {
            // Make sure we clean up everybody who is disposable, and use Aggregator.Run to isolate Dispose failures
            foreach (var disposable in _assemblyFixtureMappings.Values.OfType<IDisposable>())
                Aggregator.Run(disposable.Dispose);

            return base.BeforeTestAssemblyFinishedAsync();
        }

        protected override Task<RunSummary> RunTestCollectionAsync(
            IMessageBus messageBus,
            ITestCollection testCollection,
            IEnumerable<IXunitTestCase> testCases,
            CancellationTokenSource cancellationTokenSource)
        {
            var testCollectionRunner = new XunitTestCollectionRunnerWithAssemblyFixture(
                _assemblyFixtureMappings,
                testCollection,
                testCases,
                DiagnosticMessageSink,
                messageBus,
                TestCaseOrderer,
                new ExceptionAggregator(Aggregator),
                cancellationTokenSource);

            return testCollectionRunner.RunAsync();
        }
    }
}
