using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class SByteValueRetriever : NonNullableValueRetriever<sbyte>
    {
        public override sbyte GetValue(string value)
        {
            sbyte.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out sbyte returnValue);
            return returnValue;
        }
    }
}