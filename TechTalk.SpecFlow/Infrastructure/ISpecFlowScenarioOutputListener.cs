namespace TechTalk.SpecFlow.Infrastructure
{
    public interface ISpecFlowScenarioOutputListener
    {
        void OnMessage(string message);
        void OnAttachmentAdded(string filePath);
    }
}
