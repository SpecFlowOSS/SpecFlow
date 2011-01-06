namespace TechTalk.SpecFlow.Vs2010Integration.Tracing
{
    public interface IVisualStudioTracer
    {
        void Trace(string message, string category);
    }
}