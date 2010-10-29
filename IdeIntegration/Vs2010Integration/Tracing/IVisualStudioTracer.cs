namespace TechTalk.SpecFlow.Vs2010Integration.Tracing
{
    internal interface IVisualStudioTracer
    {
        void Trace(string message, string category);
    }
}