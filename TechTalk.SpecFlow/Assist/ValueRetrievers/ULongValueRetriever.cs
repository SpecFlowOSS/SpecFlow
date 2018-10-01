using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class ULongValueRetriever : NonNullableValueRetriever<ulong>
    {
        public override ulong GetValue(string value)
        {
            ulong.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out ulong returnValue);
            return returnValue;
        }
    }
}