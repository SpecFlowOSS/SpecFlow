using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class SByteValueRetriever : StructRetriever<sbyte>
    {
        protected override sbyte GetNonEmptyValue(string value)
        {
            sbyte.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out sbyte returnValue);
            return returnValue;
        }
    }
}