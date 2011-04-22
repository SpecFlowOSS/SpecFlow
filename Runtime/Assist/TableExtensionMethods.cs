using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TechTalk.SpecFlow.Assist
{
    public static class TableHelperExtensionMethods
    {
        public static T CreateInstance<T>(this Table table)
        {
            return ThisTypeHasADefaultConstructor<T>()
                       ? CreateTheInstanceWithTheDefaultConstructor<T>(table)
                       : CreateTheInstanceWithTheValuesFromTheTable<T>(table);
        }

        public static void FillInstance<T>(this Table table, T instance)
        {
            LoadInstanceWithKeyValuePairs(table, instance);
        }

        public static IEnumerable<T> CreateSet<T>(this Table table)
        {
            var pivotTable = new PivotTable(table);

            var list = new List<T>();

            for (var index = 0; index < table.Rows.Count(); index++)
            {
                var instance = pivotTable.GetInstanceTable(index).CreateInstance<T>();
                list.Add(instance);
            }

            return list;
        }

        private static T CreateTheInstanceWithTheDefaultConstructor<T>(Table table)
        {
            var instance = (T)Activator.CreateInstance(typeof(T));
            LoadInstanceWithKeyValuePairs(table, instance);
            return instance;
        }

        private static T CreateTheInstanceWithTheValuesFromTheTable<T>(Table table)
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

        private static bool ThisTypeHasADefaultConstructor<T>()
        {
            return typeof(T).GetConstructors()
                .Where(c => c.GetParameters().Length == 0)
                .Count() > 0;
        }

        private static ConstructorInfo GetConstructorMatchingToColumnNames<T>(Table table)
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

        private static bool IsPropertyMatchingToColumnName(PropertyInfo property, string columnName)
        {
            return property.Name.Equals(columnName.Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase);
        }

        private static void LoadInstanceWithKeyValuePairs<T>(Table table, T instance)
        {
            var propertiesThatNeedToBeSet = GetPropertiesThatNeedToBeSet<T>(table);

            propertiesThatNeedToBeSet.ToList()
                .ForEach(x => instance.SetPropertyValue(x.PropertyName, x.Handler(x.Row, x.Row["Value"])));
        }

        private static IEnumerable<PropertyHandler> GetPropertiesThatNeedToBeSet<T>(Table table)
        {
            var handlers = GetTypeHandlersForFieldValuePairs<T>();

            return from property in typeof(T).GetProperties()
                   from key in handlers.Keys
                   from row in table.Rows
                   where key.IsAssignableFrom(property.PropertyType)
                       && IsPropertyMatchingToColumnName(property, row["Field"])
                   select new PropertyHandler { Row = row, PropertyName = property.Name, Handler = handlers[key] };
        }

        private static Dictionary<Type, Func<TableRow, string, object>> GetTypeHandlersForFieldValuePairs<T>()
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

        private class PropertyHandler
        {
            public TableRow Row { get; set; }
            public string PropertyName { get; set; }
            public Func<TableRow, string, object> Handler { get; set; }
        }
    }
}