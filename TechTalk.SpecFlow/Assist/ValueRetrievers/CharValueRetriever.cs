namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class CharValueRetriever : StructRetriever<char>
    {
        protected override char GetNonEmptyValue(string value)
        {
            return value != null && value.Length == 1 
                ? value[0] 
                : '\0';
        }
    }
}