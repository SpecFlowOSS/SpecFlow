using System;
using System.Linq;

namespace TechTalk.SpecFlow.Tracing
{
    public class DefaultListener : ITraceListener
    {
        private ITraceListener traceListener;

        public void WriteTestOutput(string message)
        {
            if (traceListener == null)
                Console.WriteLine(message);
            else
                traceListener.WriteTestOutput(message);
        }

        public void WriteToolOutput(string message)
        {
            if (traceListener == null)
                Console.WriteLine("-> " + message);
            else
                traceListener.WriteToolOutput("-> " + message);
        }

        public void OverrideWith(ITraceListener traceListener)
        {
            this.traceListener = traceListener;
        }
    }
}