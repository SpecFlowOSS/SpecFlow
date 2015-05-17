namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class ULongValueRetriever
    {
        public virtual ulong GetValue(string value)
        {
            ulong returnValue;
            ulong.TryParse(value, out returnValue);
            return returnValue;
        }
    }
}