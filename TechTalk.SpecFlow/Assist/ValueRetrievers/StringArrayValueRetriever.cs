using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class StringArrayValueRetriever : IValueRetriever
    {
        public virtual string[] GetValue(string value)
        {
            const char byCommaSeparator = ',';

            var stringArray = value.Split(byCommaSeparator).Select(p => p.Trim()).ToArray();

            return stringArray;
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(string[]);
        }
    }
}