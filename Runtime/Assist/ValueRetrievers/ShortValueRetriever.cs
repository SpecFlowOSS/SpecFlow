namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class ShortValueRetriever
    {
        public virtual short GetValue(string value)
        {
            short returnValue;
            short.TryParse(value, out returnValue);
            return returnValue;
        }
    }
}