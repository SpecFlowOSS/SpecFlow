namespace TechTalk.SpecFlow.Tracing
{
    public static class TraceListenerHelper
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