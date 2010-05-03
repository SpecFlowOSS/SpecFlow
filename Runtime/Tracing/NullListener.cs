namespace TechTalk.SpecFlow.Tracing
{
    public class NullListener : ITraceListener
    {
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