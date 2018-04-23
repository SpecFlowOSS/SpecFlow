using System;
using System.Collections.Generic;
using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class DoubleValueRetriever : IValueRetriever
    {
        public virtual double GetValue(string value)
        {
	        Double.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out double returnValue);
            return returnValue;
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(double);
        }
    }
}