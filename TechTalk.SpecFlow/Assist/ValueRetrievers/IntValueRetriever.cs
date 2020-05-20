using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class IntValueRetriever : StructRetriever<int>
    {
        protected override int GetNonEmptyValue(string value)
        {
            int.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out int returnValue);
            return returnValue;
        }
    }
}