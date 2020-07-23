using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullValueRetriever : IValueRetriever
    {
        private readonly string _nullText;

        public NullValueRetriever(string nullText)
        {
            _nullText = nullText;
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return IsNullableType(propertyType) &&
                   keyValuePair.Value != null &&
                   string.Compare(keyValuePair.Value.Trim(), _nullText, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return null;
        }

        private static bool IsNullableType(Type type)
        {
            return !type.IsValueType || 
                (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
    }
}
