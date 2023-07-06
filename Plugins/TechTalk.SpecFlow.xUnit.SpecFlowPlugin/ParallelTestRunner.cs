using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;
using Xunit;

namespace TechTalk.SpecFlow.xUnit.SpecFlowPlugin
{
    public class ParallelTestClassRunner : XunitTestClassRunner
    {
        public ParallelTestClassRunner(ITestClass testClass, IReflectionTypeInfo @class, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ITestCaseOrderer testCaseOrderer, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, IDictionary<Type, object> collectionFixtureMappings)
            : base(testClass, @class, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource, collectionFixtureMappings)
        {
        }

        // This method has been slightly modified from the original implementation to run tests in parallel
        // https://github.com/xunit/xunit/blob/2.4.2/src/xunit.execution/Sdk/Frameworks/Runners/TestClassRunner.cs#L194-L219
        protected override async Task<RunSummary> RunTestMethodsAsync()
        {
            var disableParallelization = TestClass.Class.GetCustomAttributes(typeof(CollectionAttribute)).Any();

            if (disableParallelization)
                return await base.RunTestMethodsAsync().ConfigureAwait(false);

            var summary = new RunSummary();
            IEnumerable<IXunitTestCase> orderedTestCases;
            try
            {
                orderedTestCases = TestCaseOrderer.OrderTestCases(TestCases);
            }
            catch (Exception ex)
            {
                var innerEx = Unwrap(ex);
                DiagnosticMessageSink.OnMessage(new DiagnosticMessage($"Test case orderer '{TestCaseOrderer.GetType().FullName}' threw '{innerEx.GetType().FullName}' during ordering: {innerEx.Message}{Environment.NewLine}{innerEx.StackTrace}"));
                orderedTestCases = TestCases.ToList();
            }

            var constructorArguments = CreateTestClassConstructorArguments();
            var methodGroups = orderedTestCases.GroupBy(tc => tc.TestMethod, TestMethodComparer.Instance);
            var methodTasks = methodGroups.Select(m => RunTestMethodAsync(m.Key, (IReflectionMethodInfo)m.Key.Method, m, constructorArguments));
            var methodSummaries = await Task.WhenAll(methodTasks).ConfigureAwait(false);

            foreach (var methodSummary in methodSummaries)
            {
                summary.Aggregate(methodSummary);
            }

            return summary;
        }

        protected override Task<RunSummary> RunTestMethodAsync(ITestMethod testMethod, IReflectionMethodInfo method, IEnumerable<IXunitTestCase> testCases, object[] constructorArguments)
            => new ParallelTestMethodRunner(testMethod, Class, method, testCases, DiagnosticMessageSink, MessageBus, new ExceptionAggregator(Aggregator), CancellationTokenSource, constructorArguments).RunAsync();

        private static Exception Unwrap(Exception ex)
        {
            while (true)
            {
                if (ex is not TargetInvocationException tiex || tiex.InnerException == null)
                    return ex;

                ex = tiex.InnerException;
            }
        }
    }
}
