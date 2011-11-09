namespace TechTalk.SpecFlow.Specs.Drivers
{
    public class ContentFileInput : FileInput
    {
        public ContentFileInput(string fileName, string content, string folder = ".")
            : base(fileName, folder, content)
        {
        }
    }
}