using System.Diagnostics;

namespace TechTalk.SpecFlow.IdeIntegration.Tracing
{
    public class NullIdeTracer : IIdeTracer
    {
        static public readonly NullIdeTracer Instance = new NullIdeTracer();

        public void Trace(string message, string category)
        {
            Debug.WriteLine(message, "SpecFlow/" + category);
        }

        public bool IsEnabled(string category)
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}