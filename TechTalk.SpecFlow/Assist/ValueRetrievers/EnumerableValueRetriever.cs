using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public abstract class EnumerableValueRetriever : IValueRetriever
    {
        private static readonly char[] Separators = { ',', ';' };

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            var valueType = GetActualValueType(propertyType);
            return valueType != null;
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            var valueType = GetActualValueType(propertyType);
            if (valueType == null)
                return null;

            var values = new object[0];
            if (!string.IsNullOrWhiteSpace(keyValuePair.Value))
            {
                values = keyValuePair.Value
                    .Split(Separators)
                    .Select(x => x.Trim())
                    .Select(x =>
                    {
                        var kvPair = new KeyValuePair<string, string>(keyValuePair.Key, x);
                        var retriever = GetValueRetriever(kvPair, targetType, valueType);
                        return retriever?.Retrieve(kvPair, targetType, valueType);
                    })
                    .ToArray();
            }

            return BuildInstance(values, valueType);
        }

        protected abstract Type GetActualValueType(Type propertyType);

        protected abstract object BuildInstance(object[] values, Type valueType);

        private IValueRetriever GetValueRetriever(KeyValuePair<string, string> keyValuePair, Type targetType, Type valueType)
        {
            return Service.Instance.ValueRetrievers.FirstOrDefault(x => x.CanRetrieve(keyValuePair, targetType, valueType));
        }
    }
}
