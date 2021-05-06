namespace TechTalk.SpecFlow.Events
{
    public interface IExecutionEventListener
    {
        void OnEvent(IExecutionEvent executionEvent);
    }
}
