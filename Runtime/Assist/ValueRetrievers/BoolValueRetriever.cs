using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class BoolValueRetriever : IValueRetriever
    {
        public virtual bool GetValue(string value)
        {
            return value == "True" || value == "true";
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type propertyType)
        {
            return propertyType == typeof(bool);
        }
    }
}