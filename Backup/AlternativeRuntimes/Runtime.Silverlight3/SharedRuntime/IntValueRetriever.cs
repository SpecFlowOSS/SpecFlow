namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class IntValueRetriever
    {
        public virtual int GetValue(string value)
        {
            int returnValue;
            int.TryParse(value, out returnValue);
            return returnValue;
        }
    }
}