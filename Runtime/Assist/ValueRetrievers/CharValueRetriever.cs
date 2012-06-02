namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class CharValueRetriever : IValueRetriever<char>
    {
        public virtual char GetValue(string value)
        {
            char result;
            TryGetValue(value, out result);
            return result;
        }

        public bool TryGetValue(string text, out char result)
        {
            return char.TryParse(text, out result);
        }
    }
}