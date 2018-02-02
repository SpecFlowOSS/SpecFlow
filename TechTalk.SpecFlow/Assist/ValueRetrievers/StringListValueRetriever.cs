using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class StringListValueRetriever : IValueRetriever
    {
        public virtual List<string> GetValue(string value)
        {
            return new StringArrayValueRetriever().GetValue(value).ToList();
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(List<string>) || propertyType == typeof(IList<string>);
        }
    }
}