namespace TechTalk.SpecFlow.Vs2010Integration.Tracing.OutputWindow
{
    public interface IOutputWindowService
    {
        IOutputWindowPane TryGetPane(string name);
    }
}
