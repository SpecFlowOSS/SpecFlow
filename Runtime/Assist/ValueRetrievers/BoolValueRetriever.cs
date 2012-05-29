namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class BoolValueRetriever : IValueRetriever<bool>
    {
        public virtual bool GetValue(string value)
        {
            return value == "True" || value == "true";
        }
    }
}