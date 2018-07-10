﻿using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow
{
    public class TestRunner : ITestRunner
    {
        private readonly ITestExecutionEngine executionEngine;

        public int ThreadId { get; private set; }

        public TestRunner(ITestExecutionEngine executionEngine)
        {
            this.executionEngine = executionEngine;
        }

        public FeatureContext FeatureContext
        {
            get { return executionEngine.FeatureContext; }
        }

        public ScenarioContext ScenarioContext
        {
            get { return executionEngine.ScenarioContext; }
        }

        public void OnTestRunStart()
        {
            executionEngine.OnTestRunStart();
        }

        public void InitializeTestRunner(int threadId)
        {
            ThreadId = threadId;
        }

        public void OnFeatureStart(FeatureInfo featureInfo)
        {
            executionEngine.OnFeatureStart(featureInfo);
        }

        public void OnFeatureEnd()
        {
            executionEngine.OnFeatureEnd();
        }

        public void OnScenarioInitialize(ScenarioInfo scenarioInfo)
        {
            executionEngine.OnScenarioInitialize(scenarioInfo);
        }

        public void OnScenarioStart()
        {
            executionEngine.OnScenarioStart();
        }

        public void CollectScenarioErrors()
        {
            executionEngine.OnAfterLastStep();
        }

        public void OnScenarioEnd()
        {
            executionEngine.OnScenarioEnd();
        }

        public void OnTestRunEnd()
        {
            executionEngine.OnTestRunEnd();
        }

        public void Given(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            executionEngine.Step(StepDefinitionKeyword.Given, keyword, text, multilineTextArg, tableArg);
        }

        public void When(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            executionEngine.Step(StepDefinitionKeyword.When, keyword, text, multilineTextArg, tableArg);
        }

        public void Then(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            executionEngine.Step(StepDefinitionKeyword.Then, keyword, text, multilineTextArg, tableArg);
        }

        public void And(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            executionEngine.Step(StepDefinitionKeyword.And, keyword, text, multilineTextArg, tableArg);
        }

        public void But(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            executionEngine.Step(StepDefinitionKeyword.But, keyword, text, multilineTextArg, tableArg);
        }

        public void Pending()
        {
            executionEngine.Pending();
        }
    }
}
