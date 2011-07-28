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
            var instance = (T)Activator.CreateInstance(typeof(T));
            LoadInstanceWithKeyValuePairs(table, instance);
            return instance;
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

        private static bool IsPropertyMatchingToColumnName(PropertyInfo property, string columnName)
        {
            return property.Name.Equals(columnName.Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase);
        }

        private static void LoadInstanceWithKeyValuePairs<T>(Table table, T instance)
        {
            var handlers = GetTypeHandlersForFieldValuePairs<T>();

            var propertiesThatNeedToBeSet = from property in typeof(T).GetProperties()
                                            from key in handlers.Keys
                                            from row in table.Rows
                                            where key.IsAssignableFrom(property.PropertyType)
                                                && IsPropertyMatchingToColumnName(property, row["Field"])
                                            select new { Row = row, property.Name, Handler = handlers[key] };

            propertiesThatNeedToBeSet.ToList()
                .ForEach(x => instance.SetPropertyValue(x.Name, x.Handler(x.Row, x.Row["Value"])));
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
                           {typeof (Enum), (TableRow row, string id) => row.GetEnumFromSingleInstanceRow<T>()},
                           {typeof (char), (TableRow row, string id) => row.GetChar("Value")},
                           {typeof (char?), (TableRow row, string id) => string.IsNullOrEmpty(row["Value"]) ? (char?)null : row.GetChar("Value")}
                       };
        }
    }
}