namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class IntValueRetriever
    {
        public int GetValue(string value)
        {
            int returnValue;
            int.TryParse(value, out returnValue);
            return returnValue;
        }
    }
}