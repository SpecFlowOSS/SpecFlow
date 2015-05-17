namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class BoolValueRetriever
    {
        public virtual bool GetValue(string value)
        {
            return value == "True" || value == "true";
        }
    }
}