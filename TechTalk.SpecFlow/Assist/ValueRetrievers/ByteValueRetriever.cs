using System;
using System.Collections.Generic;
using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class ByteValueRetriever : IValueRetriever
    {
        public virtual byte GetValue(string value)
        {
            byte.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out byte returnValue);
			return returnValue;
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(byte);
        }
    }
}