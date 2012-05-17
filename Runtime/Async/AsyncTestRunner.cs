using System;
using System.Reflection;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Infrastructure;

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
        private readonly ITestExecutionEngine testExecutionEngine;

        public AsyncTestRunner(ITestExecutionEngine testExecutionEngine)
        {
            this.testExecutionEngine = testExecutionEngine;
        }

        public FeatureContext FeatureContext
        {
            get { return testExecutionEngine.FeatureContext; }
        }

        public ScenarioContext ScenarioContext
        {
            get { return testExecutionEngine.ScenarioContext; }
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

        // This method is called from the generated code so that it can register the IAsyncTestExecutor, which
        // is the abstraction between us and the actual implementation of the async enqueueing stuff
        static public void RegisterAsyncTestExecutor(ITestRunner testRunner, IAsyncTestExecutor newAsyncTestExecutor)
        {
            var asyncTestRunner = testRunner as AsyncTestRunner;
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
            testExecutionEngine.Initialize(bindingAssemblies);
        }

        public void OnFeatureStart(FeatureInfo featureInfo)
        {
            // No enqueueing
            // I don't like the idea of enqueueing async steps in a BeforeFeature. Feels like code/test smell
            // Also, feature setup/teardown is static, so there will be no context
            testExecutionEngine.OnFeatureStart(featureInfo);
        }

        public void OnFeatureEnd()
        {
            // No enqueueing
            // We're at the end of the async task list, so again, feels like a smell to try and enqueue something
            // And again, static method, so no context
            testExecutionEngine.OnFeatureEnd();
        }

        public void OnTestRunEnd()
        {
            // No enqueueing
            // We're at the end of the async task list, so again, feels like a smell to try and enqueue something
            // And again, static method, so no context
            testExecutionEngine.OnTestRunEnd();
        }

        public void OnScenarioStart(ScenarioInfo scenarioInfo)
        {
            if (asyncTestExecutor == null)
                throw new InvalidOperationException("Cannot start an asynchronous scenario with a null AsyncContext");

            // No enqueueing
            // The queue is logically empty at this point (we're the first thing to run in a scenario)
            // and enqueueing right now will just add to an empty list
            testExecutionEngine.OnScenarioStart(scenarioInfo);

            // register the test executor in the scenario context to be able to used AOP style
            ScenarioContext.Set(asyncTestExecutor);
        }

        public void CollectScenarioErrors()
        {
            // Make sure these commands run after all other steps
            AsyncTestExecutor.EnqueueCallback(() => testExecutionEngine.OnAfterLastStep());
            AsyncTestExecutor.EnqueueTestComplete();
        }

        public void OnScenarioEnd()
        {
            // No enqueueing
            // The test framework should ensure that we're called after the test completes
            testExecutionEngine.OnScenarioEnd();

            UnregisterAsyncTestExecutor();
        }

        public void Given(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            AsyncTestExecutor.EnqueueWithNewContext(() => testExecutionEngine.Step(StepDefinitionKeyword.Given, keyword, text, multilineTextArg, tableArg));
        }

        public void When(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            AsyncTestExecutor.EnqueueWithNewContext(() => testExecutionEngine.Step(StepDefinitionKeyword.When, keyword, text, multilineTextArg, tableArg));
        }

        public void Then(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            AsyncTestExecutor.EnqueueWithNewContext(() => testExecutionEngine.Step(StepDefinitionKeyword.Then, keyword, text, multilineTextArg, tableArg));
        }

        public void And(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            AsyncTestExecutor.EnqueueWithNewContext(() => testExecutionEngine.Step(StepDefinitionKeyword.And, keyword, text, multilineTextArg, tableArg));
        }

        public void But(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            AsyncTestExecutor.EnqueueWithNewContext(() => testExecutionEngine.Step(StepDefinitionKeyword.But, keyword, text, multilineTextArg, tableArg));
        }

        public void Pending()
        {
            AsyncTestExecutor.EnqueueWithNewContext(() => testExecutionEngine.Pending());
        }
    }
}