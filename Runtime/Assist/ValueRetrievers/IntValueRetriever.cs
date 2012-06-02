namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class IntValueRetriever : IValueRetriever<int>
    {
        public virtual int GetValue(string value)
        {
            int returnValue;
            TryGetValue(value, out returnValue);
            return returnValue;
        }

        public bool TryGetValue(string text, out int result)
        {
            return int.TryParse(text, out result);
        }
    }
}