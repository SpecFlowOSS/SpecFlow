using System;
using System.Collections.Generic;
using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class DecimalValueRetriever : IValueRetriever
    {
        public virtual decimal GetValue(string value)
        {
	        Decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out Decimal returnValue);
			return returnValue;
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(decimal);
        }
    }
}