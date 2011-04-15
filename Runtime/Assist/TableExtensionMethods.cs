using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

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
                .ForEach(x => instance.SetPropertyValue(x.Name, x.Handler(x.Row)));
        }

        private static Dictionary<Type, Func<TableRow, object>> GetTypeHandlersForFieldValuePairs<T>()
        {
            return new Dictionary<Type, Func<TableRow, object>>
                       {
                           {typeof (string), (TableRow row) => new StringValueRetriever().GetValue(row["Value"])},
                           {typeof (int), (TableRow row) => new IntValueRetriever().GetValue(row["Value"])},
                           {typeof (int?), (TableRow row) => new NullableIntValueRetriever(new IntValueRetriever()).GetValue(row["Value"])},
                           {typeof (decimal), (TableRow row) => row.GetDecimal("Value")},
                           {typeof (decimal?), (TableRow row) => string.IsNullOrEmpty(row["Value"]) ? (decimal?)null : row.GetDecimal("Value")},
                           {typeof (bool), (TableRow row) => row.GetBoolean("Value")},
                           {typeof (bool?), (TableRow row) => row["Value"] == "" ? (bool?)null : row.GetBoolean("Value")},
                           {typeof (DateTime), (TableRow row) => row.GetDateTime("Value")},
                           {typeof (DateTime?), (TableRow row) => string.IsNullOrEmpty(row["Value"]) ? (DateTime?)null :  row.GetDateTime("Value")},
                           {typeof (Double), (TableRow row) => row.GetDouble("Value")},
                           {typeof (Double?), (TableRow row) => string.IsNullOrEmpty(row["Value"]) ? (Double?)null : row.GetDouble("Value")},
                           {typeof (Guid), (TableRow row) => row.GetGuid("Value")},
                           {typeof (Guid?), (TableRow row) => string.IsNullOrEmpty(row["Value"]) ? (Guid?)null : row.GetGuid("Value")},
                           {typeof (Enum), (TableRow row) => row.GetEnum<T>("Value")},
                           {typeof (char), (TableRow row) => row.GetChar("Value")},
                           {typeof (char?), (TableRow row) => string.IsNullOrEmpty(row["Value"]) ? (char?)null : row.GetChar("Value")}
                       };
        }
    }
}