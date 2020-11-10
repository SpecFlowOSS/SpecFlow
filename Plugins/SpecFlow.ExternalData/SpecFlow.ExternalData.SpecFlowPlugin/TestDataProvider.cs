using System.IO;
using TechTalk.SpecFlow.Generator.Interfaces;
using Utf8Json;

namespace SpecFlow.ExternalData.SpecFlowPlugin
{
    public interface ITestDataProvider
    {
        dynamic TestData { get; }
    }

    public class TestDataProvider : ITestDataProvider
    {
        public TestDataProvider(ProjectSettings projectSettings)
        {
            using (var stream = File.OpenRead(Path.Combine(projectSettings.ProjectFolder, "testdata.json")))
            {
                TestData = JsonSerializer.Deserialize<dynamic>(stream);
            }
        }

        public dynamic TestData { get; }
    }
}
