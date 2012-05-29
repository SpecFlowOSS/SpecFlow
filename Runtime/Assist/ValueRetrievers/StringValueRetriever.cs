namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class StringValueRetriever : IValueRetriever<string>
    {
        public string GetValue(string value)
        {
            return value;
        }
    }
}