namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class UShortValueRetriever : IValueRetriever<ushort>
    {
        public virtual ushort GetValue(string value)
        {
            ushort returnValue;
            ushort.TryParse(value, out returnValue);
            return returnValue;
        }
    }
}