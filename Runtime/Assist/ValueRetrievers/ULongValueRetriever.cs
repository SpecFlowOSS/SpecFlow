namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class ULongValueRetriever
    {
        public virtual ulong GetValue(string value)
        {
            ulong returnValue;
            ulong.TryParse(value, out returnValue);
            return returnValue;
        }
    }
}