using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class ListValueRetriever : EnumerableValueRetriever
    {
        private MethodInfo toListMethodInfo;

        public override bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            if (!propertyType.IsGenericType)
            {
                return false;
            }

            var genericType = propertyType.GetGenericTypeDefinition();
            return genericType == typeof(List<>)
                   || genericType == typeof(IEnumerable<>)
                   || genericType == typeof(ICollection<>)
                   || genericType == typeof(IList<>)
                   || genericType == typeof(IReadOnlyList<>);
        }

        protected override Type GetActualValueType(Type propertyType)
        {
            return propertyType.GetGenericArguments()[0];
        }

        protected override object BuildInstance(int count, IEnumerable values, Type valueType)
        {
            return GetMethod().MakeGenericMethod(valueType).Invoke(this, new [] { values });
        }

        private MethodInfo GetMethod() => toListMethodInfo = toListMethodInfo ??  typeof(ListValueRetriever).GetMethod(nameof(ToList), BindingFlags.NonPublic | BindingFlags.Instance);

        private List<T> ToList<T>(IEnumerable values)
        {
            return values.Cast<T>().ToList();
        }
    }
}