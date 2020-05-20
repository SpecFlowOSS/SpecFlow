using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class LongValueRetriever : StructRetriever<long>
    {
        protected override long GetNonEmptyValue(string value)
        {
            long.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out long returnValue);
            return returnValue;
        }
    }
}