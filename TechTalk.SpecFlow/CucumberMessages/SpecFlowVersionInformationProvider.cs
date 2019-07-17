namespace TechTalk.SpecFlow.CucumberMessages
{
    public class SpecFlowVersionInformationProvider : ISpecFlowVersionInformationProvider
    {
        public string GetPackageVersion() => VersionInfo.NuGetVersion;
    }
}
