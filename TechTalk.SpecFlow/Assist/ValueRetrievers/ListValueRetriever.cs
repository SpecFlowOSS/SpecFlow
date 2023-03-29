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
                   || genericType == typeof(IReadOnlyList<>)
                   || genericType == typeof(IReadOnlyCollection<>);
        }

        protected override Type GetActualValueType(Type propertyType)
        {
            return propertyType.GetGenericArguments()[0];
        }

        protected override object BuildInstance(int count, IEnumerable values, Type valueType)
        {
            return GetMethod().MakeGenericMethod(valueType).Invoke(null, new object[] { values });
        }

        private MethodInfo GetMethod() => toListMethodInfo ??= typeof(ListValueRetriever).GetMethod(nameof(ToList), BindingFlags.NonPublic | BindingFlags.Static);

        private static List<T> ToList<T>(IEnumerable values)
        {
            return values.Cast<T>().ToList();
        }
    }
}