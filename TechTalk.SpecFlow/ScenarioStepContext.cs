using System.Diagnostics;
using System.Threading;

namespace TechTalk.SpecFlow
{
    public interface IScenarioStepContext : ISpecFlowContext
    {
        StepInfo StepInfo { get; }

        ScenarioExecutionStatus Status { get; set; }
    }

    public class ScenarioStepContext : SpecFlowContext, IScenarioStepContext
    {
        #region Singleton
        private static bool isCurrentDisabled = false;
        private static ScenarioStepContext current;
        public static ScenarioStepContext Current
        {
            get
            {
                if (isCurrentDisabled)
                    throw new SpecFlowException("The ScenarioStepContext.Current static accessor cannot be used in multi-threaded execution. Try injecting the scenario context to the binding class. See https://go.specflow.org/doc-multithreaded for details.");
                if (current == null)
                {
                    Debug.WriteLine("Accessing NULL ScenarioStepContext");
                }
                return current;
            }
            internal set
            {
                if (!isCurrentDisabled)
                    current = value;
            }
        }

        internal static void DisableSingletonInstance()
        {
            isCurrentDisabled = true;
            Thread.MemoryBarrier();
            current = null;
        }
        #endregion

        public StepInfo StepInfo { get; private set; }

        public ScenarioExecutionStatus Status { get; set; }

        internal ScenarioStepContext(StepInfo stepInfo)
        {
            StepInfo = stepInfo;
        }
    }
}