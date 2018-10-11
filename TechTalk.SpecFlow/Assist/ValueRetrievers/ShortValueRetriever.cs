using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class ShortValueRetriever : NonNullableValueRetriever<short>
    {
        public override short GetValue(string value)
        {
            short.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out short returnValue);
            return returnValue;
        }
    }
}