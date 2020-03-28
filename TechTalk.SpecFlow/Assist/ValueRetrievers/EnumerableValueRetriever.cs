using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public abstract class EnumerableValueRetriever : IValueRetriever
    {
        private static readonly char[] Separators = { ',', ';' };

        public abstract bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType);

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            var valueType = GetActualValueType(propertyType);
            if (valueType == null)
                return null;

            int count;
            IEnumerable items;
            if (string.IsNullOrWhiteSpace(keyValuePair.Value))
            {
                count = 0;
                items = Enumerable.Empty<object>();
            }
            else
            {
                var strings = keyValuePair.Value.Split(Separators);
                count = strings.Length;
                items = GetItems(strings, keyValuePair, targetType, valueType);
            }

            return BuildInstance(count, items, valueType);
        }

        private IEnumerable GetItems(string[] strings, KeyValuePair<string, string> keyValuePair, Type targetType, Type itemType)
        {
            IValueRetriever retriever = null;
            foreach (var splitValue in strings)
            {
                var itemKeyValuePair = new KeyValuePair<string, string>(keyValuePair.Key, splitValue.Trim());
                retriever = retriever ?? GetValueRetriever(itemKeyValuePair, targetType, itemType);
                yield return retriever?.Retrieve(itemKeyValuePair, targetType, itemType);
            }
        }

        protected abstract Type GetActualValueType(Type propertyType);

        protected abstract object BuildInstance(int count, IEnumerable values, Type valueType);

        private IValueRetriever GetValueRetriever(KeyValuePair<string, string> keyValuePair, Type targetType, Type valueType)
        {
            foreach (var retriever in Service.Instance.ValueRetrievers)
            {
                if (retriever.CanRetrieve(keyValuePair, targetType, valueType))
                {
                    return retriever;
                }
            }

            return null;
        }
    }
}
