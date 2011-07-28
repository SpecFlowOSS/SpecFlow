using System;
using System.Reflection;

namespace TechTalk.SpecFlow.Async
{
    /// <summary>
    /// Implements <see cref="ITestRunner"/>, enqueueing each scenario's steps to be run asynchronously
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class only supports running steps asynchronously. Feature and scenario setup/teardown remains
    /// synchronous.
    /// </para>
    /// </remarks>
    public class AsyncTestRunner : ITestRunner
    {
        private readonly ITestRunner testRunner;

        public AsyncTestRunner(ITestRunner testRunner)
        {
            this.testRunner = testRunner;
        }

        private IAsyncTestExecutor asyncTestExecutor;

        public IAsyncTestExecutor AsyncTestExecutor
        {
            get
            {
                if (asyncTestExecutor == null)
                    throw new InvalidOperationException("Cannot execute asynchronous scenario without asynch test executor");

                return asyncTestExecutor;
            }
        }

        public void UnregisterAsyncTestExecutor()
        {
            if (asyncTestExecutor != null)
            {
                asyncTestExecutor.Dispose();
                asyncTestExecutor = null;
            }
        }

        static public void RegisterAsyncTestExecutor(ITestRunner testRunner, IAsyncTestExecutor newAsyncTestExecutor)
        {
            AsyncTestRunner asyncTestRunner = testRunner as AsyncTestRunner;
            if (asyncTestRunner != null)
                asyncTestRunner.RegisterAsyncTestExecutor(newAsyncTestExecutor);
        }

        public void RegisterAsyncTestExecutor(IAsyncTestExecutor newAsyncTestExecutor)
        {
            UnregisterAsyncTestExecutor();
            asyncTestExecutor = newAsyncTestExecutor;
        }

        public void InitializeTestRunner(Assembly[] bindingAssemblies)
        {
            // Don't do anything. We assume the inner testRunner has already been initialised
        }

        public void OnFeatureStart(FeatureInfo featureInfo)
        {
            // No enqueueing
            // I don't like the idea of enqueueing async steps in a BeforeFeature. Feels like code/test smell
            // Also, feature setup/teardown is static, so there will be no context
            testRunner.OnFeatureStart(featureInfo);
        }

        public void OnFeatureEnd()
        {
            // No enqueueing
            // We're at the end of the async task list, so again, feels like a smell to try and enqueue something
            // And again, static method, so no context
            testRunner.OnFeatureEnd();
        }

        public void OnTestRunEnd()
        {
            // No enqueueing
            // We're at the end of the async task list, so again, feels like a smell to try and enqueue something
            // And again, static method, so no context
            testRunner.OnTestRunEnd();
        }

        public void OnScenarioStart(ScenarioInfo scenarioInfo)
        {
            if (asyncTestExecutor == null)
                throw new InvalidOperationException("Cannot start an asynchronous scenario with a null AsyncContext");

            // No enqueueing
            // The queue is logically empty at this point (we're the first thing to run in a scenario)
            // and enqueueing right now will just add to an empty list
            testRunner.OnScenarioStart(scenarioInfo);

            // register the test executor in the scenario context to be able to used AOP style
            ObjectContainer.ScenarioContext.Set(asyncTestExecutor);
            ObjectContainer.ScenarioContext.SetTestRunnerUnchecked(this);
        }

        public void CollectScenarioErrors()
        {
            // Make sure these commands run after all other steps
            AsyncTestExecutor.EnqueueCallback(() => testRunner.CollectScenarioErrors());
            AsyncTestExecutor.EnqueueTestComplete();
        }

        public void OnScenarioEnd()
        {
            // No enqueueing
            // The test framework should ensure that we're called after the test completes
            testRunner.OnScenarioEnd();

            UnregisterAsyncTestExecutor();
        }

        public void Given(string text, string multilineTextArg, Table tableArg)
        {
            AsyncTestExecutor.EnqueueWithNewContext(() => testRunner.Given(text, multilineTextArg, tableArg));
        }

        public void When(string text, string multilineTextArg, Table tableArg)
        {
            AsyncTestExecutor.EnqueueWithNewContext(() => testRunner.When(text, multilineTextArg, tableArg));
        }

        public void Then(string text, string multilineTextArg, Table tableArg)
        {
            AsyncTestExecutor.EnqueueWithNewContext(() => testRunner.Then(text, multilineTextArg, tableArg));
        }

        public void And(string text, string multilineTextArg, Table tableArg)
        {
            AsyncTestExecutor.EnqueueWithNewContext(() => testRunner.And(text, multilineTextArg, tableArg));
        }

        public void But(string text, string multilineTextArg, Table tableArg)
        {
            AsyncTestExecutor.EnqueueWithNewContext(() => testRunner.But(text, multilineTextArg, tableArg));
        }

        public void Pending()
        {
            AsyncTestExecutor.EnqueueWithNewContext(() => testRunner.Pending());
        }
    }
}