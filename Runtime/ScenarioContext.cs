using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BoDi;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Infrastructure;

#if SILVERLIGHT
using TechTalk.SpecFlow.Compatibility;
#endif

namespace TechTalk.SpecFlow
{
    public class ScenarioContext : SpecFlowContext
    {
        private static ScenarioContext current;
        public static ScenarioContext Current
        {
            get
            {
                if (current == null)
                {
                    Debug.WriteLine("Accessing NULL ScenarioContext");
                }
                return current;
            }
            internal set { current = value; }
        }

        public ScenarioInfo ScenarioInfo { get; private set; }
        public ScenarioBlock CurrentScenarioBlock { get; internal set; }
        public Exception TestError { get; internal set; }

        internal TestStatus TestStatus { get; set; }
        internal List<string> PendingSteps { get; private set; }
        internal List<StepInstance> MissingSteps { get; private set; }
        internal Stopwatch Stopwatch { get; private set; }

        internal ITestRunner TestRunner { get; private set; } 

        private readonly IObjectContainer scenarioContainer;

        internal IObjectContainer ScenarioContainer
        {
            get { return scenarioContainer; }
        }

        internal ScenarioContext(ScenarioInfo scenarioInfo, ITestRunner testRunner, IObjectContainer parentContainer)
        {
            this.scenarioContainer = CreateScenarioContainer(parentContainer);
            TestRunner = testRunner;

            Stopwatch = new Stopwatch();
            Stopwatch.Start();

            CurrentScenarioBlock = ScenarioBlock.None;
            ScenarioInfo = scenarioInfo;
            TestStatus = TestStatus.OK;
            PendingSteps = new List<string>();
            MissingSteps = new List<StepInstance>();
        }

        private ObjectContainer CreateScenarioContainer(IObjectContainer parentContainer)
        {
            return parentContainer == null ? new ObjectContainer() : new ObjectContainer(parentContainer);
        }

        public void Pending()
        {
            TestRunner.Pending();
        }

        //TODO[thread-safety]: remove this method and expose container
        public object GetBindingInstance(Type bindingType)
        {
            return scenarioContainer.Resolve(bindingType);
        }

        internal void SetBindingInstance(Type bindingType, object instance)
        {
            scenarioContainer.RegisterInstanceAs(instance, bindingType);
        }

        protected override void Dispose()
        {
            base.Dispose();

            scenarioContainer.Dispose();
        }
    }
}