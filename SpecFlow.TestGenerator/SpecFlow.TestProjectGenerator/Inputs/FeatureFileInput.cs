namespace SpecFlow.TestProjectGenerator.Inputs
{
    public class FeatureFileInput : FileInputWithContent
    {
        public FeatureFileInput(string fileName, string content, string folder = ".") : base(fileName, UnescapeContent(content), folder)
        {
        }

        private static string UnescapeContent(string content)
        {
            return content?.Replace("'''", "\"\"\"");
        }
    }
}