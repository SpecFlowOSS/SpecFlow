namespace SpecFlow.TestProjectGenerator.Inputs
{
    public class FileInputWithContent : FileInput
    {
        public FileInputWithContent(string fileName, string content, string folder) : base(fileName, folder)
        {
            Content = content;
        }

        public string Content { get; }
        public string SourceFilePath { get; set; }
    }
}