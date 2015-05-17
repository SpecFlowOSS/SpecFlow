namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class UShortValueRetriever
    {
        public virtual ushort GetValue(string value)
        {
            ushort returnValue;
            ushort.TryParse(value, out returnValue);
            return returnValue;
        }
    }
}