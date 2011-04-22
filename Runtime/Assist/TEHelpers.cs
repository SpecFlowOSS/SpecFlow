using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

            var propertiesThatNeedToBeSet = GetPropertiesThatNeedToBeSet<T>(table);

            var constructorParameters = constructor.GetParameters();
            var parameterValues = new object[constructorParameters.Length];
            for (var parameterIndex = 0; parameterIndex < constructorParameters.Length; parameterIndex++)
            {
                var parameterName = constructorParameters[parameterIndex].Name;
                var property = (from p in propertiesThatNeedToBeSet
                                where p.PropertyName == parameterName
                                select p).FirstOrDefault();
                if (property != null)
                    parameterValues[parameterIndex] = property.Handler(property.Row, property.Row["Value"]);
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
                                         where IsPropertyMatchingToColumnName(property, row["Field"])
                                         select property.Name;

            return (from constructor in typeof(T).GetConstructors()
                    where projectedPropertyNames.Except(
                        from parameter in constructor.GetParameters()
                        select parameter.Name).Count() == 0
                    select constructor).FirstOrDefault();
        }

        internal static bool IsPropertyMatchingToColumnName(PropertyInfo property, string columnName)
        {
            return property.Name.Equals(columnName.Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase);
        }

        internal static void LoadInstanceWithKeyValuePairs<T>(Table table, T instance)
        {
            var propertiesThatNeedToBeSet = GetPropertiesThatNeedToBeSet<T>(table);

            propertiesThatNeedToBeSet.ToList()
                .ForEach(x => instance.SetPropertyValue(x.PropertyName, x.Handler(x.Row, x.Row["Value"])));
        }

        internal static IEnumerable<PropertyHandler> GetPropertiesThatNeedToBeSet<T>(Table table)
        {
            var handlers = GetTypeHandlersForFieldValuePairs<T>();

            return from property in typeof(T).GetProperties()
                   from key in handlers.Keys
                   from row in table.Rows
                   where key.IsAssignableFrom(property.PropertyType)
                         && IsPropertyMatchingToColumnName(property, row["Field"])
                   select new PropertyHandler { Row = row, PropertyName = property.Name, Handler = handlers[key] };
        }

        internal static Dictionary<Type, Func<TableRow, string, object>> GetTypeHandlersForFieldValuePairs<T>()
        {
            return new Dictionary<Type, Func<TableRow, string, object>>
                       {
                           {typeof (string), (TableRow row, string id) => row.GetString("Value")},
                           {typeof (int), (TableRow row, string id) => row.GetInt32("Value")},
                           {typeof (int?), (TableRow row, string id) => string.IsNullOrEmpty(row["Value"]) ? (int?)null : row.GetInt32("Value")},
                           {typeof (decimal), (TableRow row, string id) => row.GetDecimal("Value")},
                           {typeof (decimal?), (TableRow row, string id) => string.IsNullOrEmpty(row["Value"]) ? (decimal?)null : row.GetDecimal("Value")},
                           {typeof (bool), (TableRow row, string id) => row.GetBoolean("Value")},
                           {typeof (bool?), (TableRow row, string id) => row["Value"] == "" ? (bool?)null : row.GetBoolean("Value")},
                           {typeof (DateTime), (TableRow row, string id) => row.GetDateTime("Value")},
                           {typeof (DateTime?), (TableRow row, string id) => string.IsNullOrEmpty(row["Value"]) ? (DateTime?)null :  row.GetDateTime("Value")},
                           {typeof (Double), (TableRow row, string id) => row.GetDouble("Value")},
                           {typeof (Double?), (TableRow row, string id) => string.IsNullOrEmpty(row["Value"]) ? (Double?)null : row.GetDouble("Value")},
                           {typeof (Guid), (TableRow row, string id) => row.GetGuid("Value")},
                           {typeof (Guid?), (TableRow row, string id) => string.IsNullOrEmpty(row["Value"]) ? (Guid?)null : row.GetGuid("Value")},
                           {typeof (Enum), (TableRow row, string id) => row.GetEnum<T>("Value")},
                           {typeof (char), (TableRow row, string id) => row.GetChar("Value")},
                           {typeof (char?), (TableRow row, string id) => string.IsNullOrEmpty(row["Value"]) ? (char?)null : row.GetChar("Value")}
                       };
        }

        internal class PropertyHandler
        {
            public TableRow Row { get; set; }
            public string PropertyName { get; set; }
            public Func<TableRow, string, object> Handler { get; set; }
        }
    }
}