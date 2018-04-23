using System;
using System.Collections.Generic;
using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class UIntValueRetriever : IValueRetriever
    {
        public virtual uint GetValue(string value)
        {
	        uint.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out uint returnValue);
			return returnValue;
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(uint);
        }
    }
}