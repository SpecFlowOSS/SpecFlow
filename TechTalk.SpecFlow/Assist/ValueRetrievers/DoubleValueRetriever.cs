using System;
using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class DoubleValueRetriever : StructRetriever<double>
    {
        protected override double GetNonEmptyValue(string value)
        {
            Double.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out double returnValue);
            return returnValue;
        }
    }
}