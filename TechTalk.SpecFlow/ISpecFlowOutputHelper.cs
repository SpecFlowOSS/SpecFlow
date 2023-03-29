namespace TechTalk.SpecFlow
{
    public interface ISpecFlowOutputHelper
    {
        void WriteLine(string message);
        void WriteLine(string format, params object[] args);
        void AddAttachment(string filePath);
    }
}