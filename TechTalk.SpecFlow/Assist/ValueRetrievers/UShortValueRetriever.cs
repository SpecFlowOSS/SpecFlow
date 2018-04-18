using System;
using System.Collections.Generic;
using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class UShortValueRetriever : IValueRetriever
    {
        public virtual ushort GetValue(string value)
        {
	        ushort.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out ushort returnValue);
			return returnValue;
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(ushort);
        }
    }
}