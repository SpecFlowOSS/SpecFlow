namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class ShortValueRetriever
    {
        public virtual short GetValue(string value)
        {
            short returnValue;
            short.TryParse(value, out returnValue);
            return returnValue;
        }
    }
}