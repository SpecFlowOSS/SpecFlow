namespace TechTalk.SpecFlow.Tests.Bindings.Drivers
{
    public class TestSettingsFileInput : FileInput
    {
        public TestSettingsFileInput(string fileName, string content, string folder = ".")
            : base(fileName, folder, content)
        {
        }
    }
}