namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class UIntValueRetriever
    {
        public virtual uint GetValue(string value)
        {
            uint returnValue;
            uint.TryParse(value, out returnValue);
            return returnValue;
        }
    }
}