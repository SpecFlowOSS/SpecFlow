using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class DateTimeValueRetriever : IValueRetriever
    {
        public virtual DateTime GetValue(string value)
        {
            var returnValue = DateTime.MinValue;
            DateTime.TryParse(value, out returnValue);
            return returnValue;
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type propertyType)
        {
            if (propertyType != typeof (DateTime)) return false;

            DateTime ignore;
            return DateTime.TryParse(keyValuePair.Value, out ignore);
        }
    }
}