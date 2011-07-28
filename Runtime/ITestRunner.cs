using System;
using System.Reflection;

namespace TechTalk.SpecFlow
{
    public interface ITestRunner
    {
        void InitializeTestRunner(Assembly[] bindingAssemblies);

        void OnFeatureStart(FeatureInfo featureInfo);
        void OnFeatureEnd();
        void OnScenarioStart(ScenarioInfo scenarioInfo);
        void CollectScenarioErrors();
        void OnScenarioEnd();
        void OnTestRunEnd();

        void Given(string text, string multilineTextArg, Table tableArg);
        void When(string text, string multilineTextArg, Table tableArg);
        void Then(string text, string multilineTextArg, Table tableArg);
        void And(string text, string multilineTextArg, Table tableArg);
        void But(string text, string multilineTextArg, Table tableArg);

        void Pending();
    }

    public static class TestRunnerDefaultArguments
    {
        public static void Given(this ITestRunner testRunner, string text)
        {
            testRunner.Given(text, null, null);
        }

        public static void Given(this ITestRunner testRunner, string text, string multilineTextArg)
        {
            testRunner.Given(text, multilineTextArg, null);
        }

        public static void When(this ITestRunner testRunner, string text)
        {
            testRunner.When(text, null, null);
        }

        public static void When(this ITestRunner testRunner, string text, string multilineTextArg)
        {
            testRunner.When(text, multilineTextArg, null);
        }

        public static void Then(this ITestRunner testRunner, string text)
        {
            testRunner.Then(text, null, null);
        }

        public static void Then(this ITestRunner testRunner, string text, string multilineTextArg)
        {
            testRunner.Then(text, multilineTextArg, null);
        }

        public static void And(this ITestRunner testRunner, string text)
        {
            testRunner.And(text, null, null);
        }

        public static void And(this ITestRunner testRunner, string text, string multilineTextArg)
        {
            testRunner.And(text, multilineTextArg, null);
        }
 
        public static void But(this ITestRunner testRunner, string text)
        {
            testRunner.But(text, null, null);
        }

        public static void But(this ITestRunner testRunner, string text, string multilineTextArg)
        {
            testRunner.But(text, multilineTextArg, null);
        }
    }
}