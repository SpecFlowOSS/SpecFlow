namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class SByteValueRetriever : IValueRetriever
    {
        public virtual sbyte GetValue(string value)
        {
            sbyte returnValue;
            sbyte.TryParse(value, out returnValue);
            return returnValue;
        }
    }
}