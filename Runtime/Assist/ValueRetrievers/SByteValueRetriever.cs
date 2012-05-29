namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class SByteValueRetriever : IValueRetriever<sbyte>
    {
        public virtual sbyte GetValue(string value)
        {
            sbyte returnValue;
            TryGetValue(value, out returnValue);
            return returnValue;
        }

        public bool TryGetValue(string text, out sbyte result)
        {
            return sbyte.TryParse(text, out result);
        }
    }
}