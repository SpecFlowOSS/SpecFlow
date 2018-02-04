namespace SpecFlow.TestProjectGenerator.Inputs
{
    public class ContentFileInput : FileInputWithContent
    {
        public ContentFileInput(string fileName, string content, string folder = ".")
            : base(fileName, content, folder)
        {
        }
    }
}