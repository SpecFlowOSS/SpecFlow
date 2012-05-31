using System;
using System.Reflection;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow
{
    public interface ITestRunner
    {
        FeatureContext FeatureContext { get; }
        ScenarioContext ScenarioContext { get; }

        void InitializeTestRunner(Assembly[] bindingAssemblies);

        void OnFeatureStart(FeatureInfo featureInfo);
        void OnFeatureEnd();
        void OnScenarioStart(ScenarioInfo scenarioInfo);
        void CollectScenarioErrors();
        void OnScenarioEnd();
        void OnTestRunEnd();

        void Given(string text, string multilineTextArg, Table tableArg, string keyword = null);
        void When(string text, string multilineTextArg, Table tableArg, string keyword = null);
        void Then(string text, string multilineTextArg, Table tableArg, string keyword = null);
        void And(string text, string multilineTextArg, Table tableArg, string keyword = null);
        void But(string text, string multilineTextArg, Table tableArg, string keyword = null);

        void Pending();
    }

    public static class TestRunnerDefaultArguments
    {
        public static void Given(this ITestRunner testRunner, string text)
        {
            testRunner.Given(text, null, null, null);
        }

        public static void When(this ITestRunner testRunner, string text)
        {
            testRunner.When(text, null, null, null);
        }

        public static void Then(this ITestRunner testRunner, string text)
        {
            testRunner.Then(text, null, null, null);
        }

        public static void And(this ITestRunner testRunner, string text)
        {
            testRunner.And(text, null, null, null);
        }

        public static void But(this ITestRunner testRunner, string text)
        {
            testRunner.But(text, null, null, null);
        }
    }
}