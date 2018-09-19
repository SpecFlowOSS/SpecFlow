namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class StringValueRetriever : NonNullableValueRetriever<string>
    {
        public override string GetValue(string value)
        {
            return value;
        }
    }
}