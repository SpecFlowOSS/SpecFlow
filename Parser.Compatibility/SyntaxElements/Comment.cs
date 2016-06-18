namespace TechTalk.SpecFlow.Parser.SyntaxElements
{
    public class Comment
    {
        public string Text { get; set; }
        public FilePosition FilePosition { get; set; }

        public Comment()
        {
        }

        public Comment(string text, FilePosition filePosition)
        {
            FilePosition = filePosition;
            Text = text;
        }
    }
}