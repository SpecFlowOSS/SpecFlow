namespace SpecFlow.TestProjectGenerator.Inputs
{
    public class CodeFileInput : FileInputWithContent
    {
        public CodeFileInput(string fileName, string folder, string content) : base(fileName, content, folder)
        {
        }
    }
}