namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class ByteValueRetriever : IValueRetriever<byte>
    {
        public virtual byte GetValue(string value)
        {
            byte returnValue;
            TryGetValue(value, out returnValue);
            return returnValue;
        }

        public bool TryGetValue(string text, out byte result)
        {
            return byte.TryParse(text, out result);
        }
    }
}