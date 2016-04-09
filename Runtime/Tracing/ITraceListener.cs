using System;
using System.Linq;

namespace TechTalk.SpecFlow.Tracing
{
    public interface ITraceListener
    {
        void WriteTestOutput(string message);
        void WriteToolOutput(string message);
    }

    /// <summary>
    /// Marker interface for trace listener that do not need queued execution
    /// </summary>
    public interface IThreadSafeTraceListener : ITraceListener
    {
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
