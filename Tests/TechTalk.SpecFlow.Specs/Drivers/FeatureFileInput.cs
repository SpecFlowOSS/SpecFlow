namespace TechTalk.SpecFlow.Specs.Drivers
{
    public class FeatureFileInput : FileInput
    {
        public FeatureFileInput(string fileName, string content, string folder = ".") : base(fileName, folder, content)
        {
        }
    }
}