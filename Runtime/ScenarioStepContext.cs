using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace TechTalk.SpecFlow
{
    public class ScenarioStepContext : SpecFlowContext
    {
        #region Singleton
        public static ScenarioStepContext Current => TestRunnerManager.GetTestRunner(Assembly.GetCallingAssembly()).ScenarioContext.StepContext;

        #endregion

        public StepInfo StepInfo { get; private set; }


        internal ScenarioStepContext(StepInfo stepInfo)
        {
            StepInfo = stepInfo;
        }
    }
}