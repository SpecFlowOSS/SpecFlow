namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class BoolValueRetriever : StructRetriever<bool>
    {
        protected override bool GetNonEmptyValue(string value)
        {
            return value == "True" || value == "true" || value == "1";
        }
    }
}