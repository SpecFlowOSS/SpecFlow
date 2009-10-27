using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TechTalk.SpecFlow
{
    internal enum TestStatus
    {
        OK,
        StepDefinitionPending,
        MissingStepDefinition,
        BindingError,
        TestError
    }

    public class ScenarioContext : SpecFlowContext
    {
        static public ScenarioContext Current
        {
            get { return ObjectContainer.ScenarioContext; }
        }

        public ScenarioInfo ScenarioInfo { get; private set; }

        public ScenarioBlock CurrentScenarioBlock { get; internal set; }

        internal TestStatus TestStatus { get; set; }
        internal Exception TestError { get; set; }
        internal List<string> PendingSteps { get; private set; }
        internal List<string> MissingSteps { get; private set; }
        internal Stopwatch Stopwatch { get; private set; }

        public ScenarioContext(ScenarioInfo scenarioInfo)
        {
            Stopwatch = new Stopwatch();
            Stopwatch.Start();

            CurrentScenarioBlock = ScenarioBlock.None;
            ScenarioInfo = scenarioInfo;
            TestStatus = TestStatus.OK;
            PendingSteps = new List<string>();
            MissingSteps = new List<string>();
        }

        public void Pending()
        {
            ObjectContainer.TestRunner.Pending();
        }

        private Dictionary<Type, object> bindingInstances = new Dictionary<Type, object>();

        public object GetBindingInstance(Type bindingType)
        {
            object value;
            if (!bindingInstances.TryGetValue(bindingType, out value))
            {
                value = Activator.CreateInstance(bindingType);
                bindingInstances.Add(bindingType, value);
            }

            return value;
        }
    }
}