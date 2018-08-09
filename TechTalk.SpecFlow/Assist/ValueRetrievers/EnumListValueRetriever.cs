using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class EnumListValueRetriever : IValueRetriever
    {
        public virtual object GetValue(string value, Type targetEnumType)
        {
            var array = new EnumArrayValueRetriever().GetValue(value, targetEnumType);
            return GenerateList(targetEnumType, array);
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value, propertyType.GetGenericArguments()[0]);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType.IsGenericType && (
                       propertyType.GetGenericTypeDefinition() == typeof(IList<>) ||
                       propertyType.GetGenericTypeDefinition() == typeof(List<>)) &&
                   propertyType.GetGenericArguments()[0].IsEnum;
        }

        object GenerateList(Type targetType, object array)
        {
            var castMethod = typeof(Enumerable).GetMethod("Cast", BindingFlags.Static | BindingFlags.Public);
            var genericCast = castMethod.MakeGenericMethod(targetType);

            var toListMethod = typeof(Enumerable).GetMethod("ToList", BindingFlags.Static | BindingFlags.Public);
            var genericToList = toListMethod.MakeGenericMethod(targetType);

            var casted = genericCast.Invoke(null, new object[] { array });
            var list = genericToList.Invoke(null, new[] { casted });

            return list;
        }
    }
}