using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Infrastructure
{
    public class SpecFlowScenarioOutputTracer : ISpecFlowScenarioOutputListener
    {
        private readonly ITraceListener _traceListener;

        public SpecFlowScenarioOutputTracer(ITraceListener traceListener)
        {
            _traceListener = traceListener;
        }

        public void OnMessage(string message)
        {
            _traceListener.WriteTestOutput(message);
        }
    }
}
