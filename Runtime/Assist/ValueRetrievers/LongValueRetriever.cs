namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class LongValueRetriever
    {
        public virtual long GetValue(string value)
        {
            long returnValue;
            long.TryParse(value, out returnValue);
            return returnValue;
        }
    }
}