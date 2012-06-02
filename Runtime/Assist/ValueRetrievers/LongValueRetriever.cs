namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class LongValueRetriever : IValueRetriever<long>
    {
        public virtual long GetValue(string value)
        {
            long returnValue;
            TryGetValue(value, out returnValue);
            return returnValue;
        }

        public bool TryGetValue(string text, out long result)
        {
            return long.TryParse(text, out result);
        }
    }
}