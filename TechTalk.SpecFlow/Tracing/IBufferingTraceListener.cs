namespace TechTalk.SpecFlow.Tracing
{
    public interface IBufferingTraceListener : ITraceListener
    {
        void BufferingStart();
        void BufferingFlushAndStop();
        void BufferingDiscardAndStop();
    }
}
