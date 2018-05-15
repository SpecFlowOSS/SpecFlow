namespace TechTalk.SpecFlow.Infrastructure
{
    public interface ISpecFlowOutputHelper
    {
        void WriteLine(string message);
        void WriteLine(string format, params object[] args);
    }
}