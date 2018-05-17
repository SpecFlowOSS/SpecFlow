using System;
using System.Collections.Generic;
using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class FloatValueRetriever : IValueRetriever
    {
        public virtual float GetValue(string value)
        {
	        float.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out float returnValue);
            return returnValue;
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(float);
        }
    }
}