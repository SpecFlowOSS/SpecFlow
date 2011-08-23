using System;
using System.Linq;
using System.Reflection;
using BoDi;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow
{
    public class TestRunner : ITestRunner
    {
        private readonly ITestExecutionEngine executionEngine;

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

        public void InitializeTestRunner(Assembly[] bindingAssemblies)
        {
            executionEngine.Initialize(bindingAssemblies);
        }

        public void OnFeatureStart(FeatureInfo featureInfo)
        {
            executionEngine.OnFeatureStart(featureInfo);
        }

        public void OnFeatureEnd()
        {
            executionEngine.OnFeatureEnd();
        }

        public void OnScenarioStart(ScenarioInfo scenarioInfo)
        {
            executionEngine.OnScenarioStart(scenarioInfo);
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

        public void Given(string text, string multilineTextArg, Table tableArg)
        {
            executionEngine.Step(StepDefinitionKeyword.Given, text, multilineTextArg, tableArg);
        }

        public void When(string text, string multilineTextArg, Table tableArg)
        {
            executionEngine.Step(StepDefinitionKeyword.When, text, multilineTextArg, tableArg);
        }

        public void Then(string text, string multilineTextArg, Table tableArg)
        {
            executionEngine.Step(StepDefinitionKeyword.Then, text, multilineTextArg, tableArg);
        }

        public void And(string text, string multilineTextArg, Table tableArg)
        {
            executionEngine.Step(StepDefinitionKeyword.And, text, multilineTextArg, tableArg);
        }

        public void But(string text, string multilineTextArg, Table tableArg)
        {
            executionEngine.Step(StepDefinitionKeyword.But, text, multilineTextArg, tableArg);
        }

        public void Pending()
        {
            executionEngine.Pending();
        }
    }
}
