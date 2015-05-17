namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class UIntValueRetriever
    {
        public virtual uint GetValue(string value)
        {
            uint returnValue;
            uint.TryParse(value, out returnValue);
            return returnValue;
        }
    }
}