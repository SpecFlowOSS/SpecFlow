namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class ShortValueRetriever : IValueRetriever<short>
    {
        public virtual short GetValue(string value)
        {
            short returnValue;
            TryGetValue(value, out returnValue);
            return returnValue;
        }

        public bool TryGetValue(string text, out short result)
        {
            return short.TryParse(text, out result);
        }
    }
}