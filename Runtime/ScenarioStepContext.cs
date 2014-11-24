namespace TechTalk.SpecFlow
{
    using System;
    using System.Diagnostics;

    using TechTalk.SpecFlow.Infrastructure;

    public class ScenarioStepContext : SpecFlowContext
    {
        public StepInfo StepInfo { get; private set; }

        private static ScenarioStepContext current;

        internal ScenarioStepContext(StepInfo stepInfo)
        {
            this.StepInfo = stepInfo;
        }

        public static ScenarioStepContext Current
        {
            get
            {
                if (current == null)
                {
                    Debug.WriteLine("Accessing NULL ScenarioStepContext");
                }
                return current;
            }
            internal set { current = value; }
        }
    }
}