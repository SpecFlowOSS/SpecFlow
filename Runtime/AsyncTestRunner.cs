using System;
using System.Reflection;

namespace TechTalk.SpecFlow
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

        private static AsyncContext AsyncContext
        {
            get { return AsyncContext.Current; }
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
            ObjectContainer.CurrentTestRunner = this;
        }

        public void OnFeatureEnd()
        {
            // No enqueueing
            // We're at the end of the async task list, so again, feels like a smell to try and enqueue something
            // And again, static method, so no context
            testRunner.OnFeatureEnd();
            ObjectContainer.CurrentTestRunner = null;
        }

        public void OnScenarioStart(ScenarioInfo scenarioInfo)
        {
            if (ObjectContainer.AsyncContext == null)
                throw new InvalidOperationException("Cannot start an asynchronous scenario with a null AsyncContext");

            // No enqueueing
            // The queue is logically empty at this point (we're the first thing to run in a scenario)
            // and enqueueing right now will just add to an empty list
            testRunner.OnScenarioStart(scenarioInfo);
        }

        public void CollectScenarioErrors()
        {
            // Make sure these commands run after all other steps
            AsyncContext.EnqueueCallback(() => testRunner.CollectScenarioErrors());
            AsyncContext.EnqueueTestComplete();
        }

        public void OnScenarioEnd()
        {
            // No enqueueing
            // The test framework should ensure that we're called after the test completes
            testRunner.OnScenarioEnd();

            AsyncContext.Unregister();
        }

        public void Given(string text, string multilineTextArg, Table tableArg)
        {
            AsyncContext.EnqueueWithNewContext(() => testRunner.Given(text, multilineTextArg, tableArg));
        }

        public void When(string text, string multilineTextArg, Table tableArg)
        {
            AsyncContext.EnqueueWithNewContext(() => testRunner.When(text, multilineTextArg, tableArg));
        }

        public void Then(string text, string multilineTextArg, Table tableArg)
        {
            AsyncContext.EnqueueWithNewContext(() => testRunner.Then(text, multilineTextArg, tableArg));
        }

        public void And(string text, string multilineTextArg, Table tableArg)
        {
            AsyncContext.EnqueueWithNewContext(() => testRunner.And(text, multilineTextArg, tableArg));
        }

        public void But(string text, string multilineTextArg, Table tableArg)
        {
            AsyncContext.EnqueueWithNewContext(() => testRunner.But(text, multilineTextArg, tableArg));
        }

        public void Pending()
        {
            AsyncContext.EnqueueWithNewContext(() => testRunner.Pending());
        }
    }
}