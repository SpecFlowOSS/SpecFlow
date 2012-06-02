namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class ULongValueRetriever : IValueRetriever<ulong>
    {
        public virtual ulong GetValue(string value)
        {
            ulong returnValue;
            TryGetValue(value, out returnValue);
            return returnValue;
        }

        public bool TryGetValue(string text, out ulong result)
        {
            return ulong.TryParse(text, out result);
        }
    }
}