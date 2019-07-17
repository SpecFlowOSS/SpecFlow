namespace TechTalk.SpecFlow.CucumberMessages
{
    public interface ISystemInformationProvider
    {
        string GetCpuArchitecture();
        string GetOperatingSystem();
    }
}
