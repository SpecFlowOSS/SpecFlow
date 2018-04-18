using System;
using System.Collections.Generic;
using System.Globalization;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class DateTimeOffsetValueRetriever : IValueRetriever
    {
        public virtual DateTimeOffset GetValue(string value)
        {
            DateTimeOffset.TryParse(value, CultureInfo.CurrentCulture,DateTimeStyles.None, out DateTimeOffset returnValue);
			return returnValue;
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(DateTimeOffset);
        }
    }
}