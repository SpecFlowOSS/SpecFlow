namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class UIntValueRetriever : IValueRetriever<uint>
    {
        public virtual uint GetValue(string value)
        {
            uint returnValue;
            TryGetValue(value, out returnValue);
            return returnValue;
        }

        public bool TryGetValue(string text, out uint result)
        {
            return uint.TryParse(text, out result);
        }
    }
}