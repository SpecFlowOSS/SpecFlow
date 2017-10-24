using System.Configuration;

namespace SpecFlow.TestProjectGenerator
{
    public class AppConfigDriver
    {
        public string ProjectName => ConfigurationManager.AppSettings["testProjectFolder"] ?? "TestProject";
    }
}