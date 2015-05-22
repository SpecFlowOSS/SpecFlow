using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.Assist
{
    internal static class TEHelpers
    {
        internal static T CreateTheInstanceWithTheDefaultConstructor<T>(Table table)
        {
            var instance = (T)Activator.CreateInstance(typeof(T));
            LoadInstanceWithKeyValuePairs(table, instance);
            return instance;
        }

        internal static T CreateTheInstanceWithTheValuesFromTheTable<T>(Table table)
        {
            var constructor = GetConstructorMatchingToColumnNames<T>(table);
            if (constructor == null)
                throw new MissingMethodException(string.Format("Unable to find a suitable constructor to create instance of {0}", typeof(T).Name));

            var membersThatNeedToBeSet = GetMembersThatNeedToBeSet(table, typeof(T));

            var constructorParameters = constructor.GetParameters();
            var parameterValues = new object[constructorParameters.Length];
            for (var parameterIndex = 0; parameterIndex < constructorParameters.Length; parameterIndex++)
            {
                var parameterName = constructorParameters[parameterIndex].Name;
                var member = (from m in membersThatNeedToBeSet
                                where m.MemberName == parameterName
                                select m).FirstOrDefault();
                if (member != null)
                    parameterValues[parameterIndex] = member.Handler(member.Row);
            }
            return (T)constructor.Invoke(parameterValues);
        }

        internal static bool ThisTypeHasADefaultConstructor<T>()
        {
            return typeof(T).GetConstructors()
                       .Where(c => c.GetParameters().Length == 0)
                       .Count() > 0;
        }

        internal static ConstructorInfo GetConstructorMatchingToColumnNames<T>(Table table)
        {
            var projectedPropertyNames = from property in typeof(T).GetProperties()
                                         from row in table.Rows
                                         where IsMemberMatchingToColumnName(property, row.Id())
                                         select property.Name;

            return (from constructor in typeof(T).GetConstructors()
                    where projectedPropertyNames.Except(
                        from parameter in constructor.GetParameters()
                        select parameter.Name).Count() == 0
                    select constructor).FirstOrDefault();
        }

        internal static bool IsMemberMatchingToColumnName(MemberInfo member, string columnName)
        {
            return member.Name.MatchesThisColumnName(columnName);
        }

        internal static bool MatchesThisColumnName(this string propertyName, string columnName)
        {
            return propertyName.Equals(columnName.Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase);

        }

        internal static void LoadInstanceWithKeyValuePairs(Table table, object instance)
        {
            var membersThatNeedToBeSet = GetMembersThatNeedToBeSet(table, instance.GetType());

            membersThatNeedToBeSet.ToList()
                .ForEach(x => x.Setter(instance, x.Handler(x.Row)));
        }

        internal static IEnumerable<MemberHandler> GetMembersThatNeedToBeSet(Table table, Type type)
        {
            var handlers = GetTypeHandlersForFieldValuePairs(type);

            var properties = from property in type.GetProperties()
                             from key in handlers.Keys
                             from row in table.Rows
                             where TheseTypesMatch(property.PropertyType, key)
                                   && IsMemberMatchingToColumnName(property, row.Id())
                             select new MemberHandler { Row = row, MemberName = property.Name, Handler = handlers[key], Setter = (i, v) => property.SetValue(i, v, null) };

            var fields = from field in type.GetFields()
                             from key in handlers.Keys
                             from row in table.Rows
                             where TheseTypesMatch(field.FieldType, key)
                                   && IsMemberMatchingToColumnName(field, row.Id())
                             select new MemberHandler { Row = row, MemberName = field.Name, Handler = handlers[key], Setter = (i, v) => field.SetValue(i, v) };

            var memberHandlers = new List<MemberHandler>();

            memberHandlers.AddRange(properties);
            memberHandlers.AddRange(fields);

            return memberHandlers;
        }

        private static bool TheseTypesMatch(Type memberType, Type handlerType)
        {
            if (handlerType.IsAssignableFrom(memberType))
                return true;
            if (memberType.IsGenericType && memberType.GetGenericTypeDefinition() == typeof(Nullable<>))
                return handlerType.IsAssignableFrom(memberType.GetGenericArguments()[0]);
            return false;
        }

        internal static Dictionary<Type, Func<TableRow, object>> GetTypeHandlersForFieldValuePairs(Type type)
        {
            return Service.Instance.ValueRetrieversByType.Keys
                .Select(x => new KeyValuePair<Type, Func<TableRow, object>>(x,
                    (TableRow row) => Service.Instance.ValueRetrieversByType[x](row, type))
                )
                .ToDictionary(x => x.Key, x => x.Value);
        }

        internal class MemberHandler
        {
            public TableRow Row { get; set; }
            public string MemberName { get; set; }
            public Func<TableRow, object> Handler { get; set; }
            public Action<object, object> Setter { get; set; }
        }

        internal static Table GetTheProperInstanceTable(Table table, Type type)
        {
            return ThisIsAVerticalTable(table, type)
                       ? table
                       : FlipThisHorizontalTableToAVerticalTable(table);
        }

        private static Table FlipThisHorizontalTableToAVerticalTable(Table table)
        {
            return new PivotTable(table).GetInstanceTable(0);
        }

        private static bool ThisIsAVerticalTable(Table table, Type type)
        {
            if (TheHeaderIsTheOldFieldValuePair(table))
                return true;
            return (table.Rows.Count() != 1) || (table.Header.Count() == 2 && TheFirstRowValueIsTheNameOfAProperty(table, type));
        }

        private static bool TheHeaderIsTheOldFieldValuePair(Table table)
        {
            return table.Header.Count() == 2 && table.Header.First() == "Field" && table.Header.Last() == "Value";
        }

        private static bool TheFirstRowValueIsTheNameOfAProperty(Table table, Type type)
        {
            var firstRowValue = table.Rows[0][table.Header.First()];
            return type.GetProperties()
                .Any(property => IsMemberMatchingToColumnName(property, firstRowValue));
        }
    }
}
