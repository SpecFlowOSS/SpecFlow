namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class BoolValueRetriever
    {
        public virtual bool GetValue(string value)
        {
            return value == "True" || value == "true";
        }
    }
}