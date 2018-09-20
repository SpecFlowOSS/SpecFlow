using System;
using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class DecimalValueRetriever : NonNullableValueRetriever<decimal>
    {
        public override decimal GetValue(string value)
        {
            Decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out Decimal returnValue);
            return returnValue;
        }
    }
}