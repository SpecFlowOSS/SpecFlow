using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class LongValueRetriever : NonNullableValueRetriever<long>
    {
        public override long GetValue(string value)
        {
            long.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out long returnValue);
            return returnValue;
        }
    }
}