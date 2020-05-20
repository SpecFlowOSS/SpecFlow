using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class ULongValueRetriever : StructRetriever<ulong>
    {
        protected override ulong GetNonEmptyValue(string value)
        {
            ulong.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out ulong returnValue);
            return returnValue;
        }
    }
}