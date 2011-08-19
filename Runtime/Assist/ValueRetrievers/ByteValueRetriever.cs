namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class ByteValueRetriever
    {
        public virtual byte GetValue(string value)
        {
            byte returnValue;
            byte.TryParse(value, out returnValue);
            return returnValue;
        }
    }
}