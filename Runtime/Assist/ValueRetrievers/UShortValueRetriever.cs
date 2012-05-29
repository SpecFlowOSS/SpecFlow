namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class UShortValueRetriever : IValueRetriever<ushort>
    {
        public virtual ushort GetValue(string value)
        {
            ushort returnValue;
            TryGetValue(value, out returnValue);
            return returnValue;
        }

        public bool TryGetValue(string text, out ushort result)
        {
            return ushort.TryParse(text, out result);
        }
    }
}