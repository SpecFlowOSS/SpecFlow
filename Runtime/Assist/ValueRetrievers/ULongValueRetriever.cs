using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class ULongValueRetriever : IValueRetriever
    {
        public virtual ulong GetValue(string value)
        {
            ulong returnValue;
            ulong.TryParse(value, out returnValue);
            return returnValue;
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type propertyType)
        {
            return propertyType == typeof(ulong);
        }
    }
}