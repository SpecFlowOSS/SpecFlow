namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class BoolValueRetriever : IValueRetriever<bool>
    {
        public virtual bool GetValue(string value)
        {
            bool returnValue;
            TryGetValue(value, out returnValue);
            return returnValue;
        }

        public bool TryGetValue(string text, out bool result)
        {
            return bool.TryParse(text, out result);
        }
    }
}