using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow.Tracing
{
    public interface ITraceListener
    {
        void WriteTestOutput(string message);
        void WriteToolOutput(string message);
    }

    static public class TraceListenerHelper
    {
        public static void WriteTestOutput(this ITraceListener traceListener, string messageFormat, params object[] args)
        {
            traceListener.WriteTestOutput(string.Format(messageFormat, args));
        }
        public static void WriteToolOutput(this ITraceListener traceListener, string messageFormat, params object[] args)
        {
            traceListener.WriteToolOutput(string.Format(messageFormat, args));
        }
    }
}
