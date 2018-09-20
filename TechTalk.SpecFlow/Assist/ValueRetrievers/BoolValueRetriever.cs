namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class BoolValueRetriever : NonNullableValueRetriever<bool>
    {
        public override bool GetValue(string value)
        {
            return value == "True" || value == "true" || value == "1";
        }
    }
}