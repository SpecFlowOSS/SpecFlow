namespace TechTalk.SpecFlow.CucumberMessages
{
    public class SpecFlowVersionInformationProvider : ISpecFlowVersionInformationProvider
    {
        public string GetAssemblyVersion()
        {
            var version = typeof(SpecFlowVersionInformationProvider).Assembly.GetName().Version;
            return $"{version.Major}.{version.Minor}";
        }
    }
}
