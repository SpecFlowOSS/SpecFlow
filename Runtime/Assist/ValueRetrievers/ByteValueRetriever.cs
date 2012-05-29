namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class ByteValueRetriever : IValueRetriever<byte>
    {
        public virtual byte GetValue(string value)
        {
            byte returnValue;
            byte.TryParse(value, out returnValue);
            return returnValue;
        }
    }
}