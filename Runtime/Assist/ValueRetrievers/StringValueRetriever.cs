namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class StringValueRetriever : IValueRetriever<string>
    {
        public string GetValue(string value)
        {
            return value;
        }

        public bool TryGetValue(string text, out string result)
        {
            result = text;
            return true;
        }
    }
}