using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class ListValueRetriever : EnumerableValueRetriever
    {
        private static readonly Type[] GenericTypes = { typeof(IEnumerable<>), typeof(ICollection<>), typeof(IList<>), typeof(List<>) };

        protected override Type GetActualValueType(Type propertyType)
        {
            if (propertyType.IsGenericType)
            {
                var definiton = propertyType.GetGenericTypeDefinition();
                if (GenericTypes.Any(x => x == definiton))
                    return propertyType.GetGenericArguments()[0];
            }
            return null;
        }

        protected override object BuildInstance(object[] values, Type valueType)
        {
            var castMethod = typeof(Enumerable).GetMethod("Cast", BindingFlags.Static | BindingFlags.Public);
            var genericCast = castMethod.MakeGenericMethod(valueType);

            var toListMethod = typeof(Enumerable).GetMethod("ToList", BindingFlags.Static | BindingFlags.Public);
            var genericToList = toListMethod.MakeGenericMethod(valueType);

            var casted = genericCast.Invoke(null, new object[] { values });
            var list = genericToList.Invoke(null, new[] { casted });

            return list;
        }
    }
}