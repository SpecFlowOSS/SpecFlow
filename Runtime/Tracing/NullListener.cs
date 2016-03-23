using System;

namespace TechTalk.SpecFlow.Tracing
{
    public class NullListener : ITraceListener, IThreadSafeTraceListener
    {
        public void SetTestname(string name)
        {
            throw new NotImplementedException();
        }

        public void WriteTestOutput(string message)
        {
            //nop;
        }

        public void WriteToolOutput(string message)
        {
            //nop;
        }
    }
}