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

        private static void LoadInstanceWithPropertyData<T>(Table table, T instance, TableRow row)
        {
            foreach (var key in GetTypeHandlersForProperties<T>().Keys)
                SetData(table, instance, row, key);
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
                           {typeof (int?), (TableRow row, string id) => row.GetInt32("Value")},
                           {typeof (decimal), (TableRow row, string id) => row.GetDecimal("Value")},
                           {typeof (decimal?), (TableRow row, string id) => row.GetDecimal("Value")},
                           {typeof (bool), (TableRow row, string id) => row.GetBoolean("Value")},
                           {typeof (bool?), (TableRow row, string id) => row.GetBoolean("Value")},
                           {typeof (DateTime), (TableRow row, string id) => row.GetDateTime("Value")},
                           {typeof (DateTime?), (TableRow row, string id) => row.GetDateTime("Value")},
                           {typeof (Double), (TableRow row, string id) => row.GetDouble("Value")},
                           {typeof (Double?), (TableRow row, string id) => row.GetDouble("Value")},
                           {typeof (Guid), (TableRow row, string id) => row.GetGuid("Value")},
                           {typeof (Guid?), (TableRow row, string id) => row.GetGuid("Value")},
                           {typeof (Enum), (TableRow row, string id) => row.GetEnum<T>("Value")},
                           {typeof (char), (TableRow row, string id) => row.GetChar("Value")},
                           {typeof (char?), (TableRow row, string id) => row.GetChar("Value")}
                       };
        }

        private static Dictionary<Type, Func<TableRow, string, object>> GetTypeHandlersForProperties<T>()
        {
            return new Dictionary<Type, Func<TableRow, string, object>>
                       {
                           {typeof (string), (TableRow row, string id) => row.GetString(id)},
                           {typeof (int), (TableRow row, string id) => row.GetInt32(id)},
                           {typeof (int?), (TableRow row, string id) => row.GetInt32(id)},
                           {typeof (decimal), (TableRow row, string id) => row.GetDecimal(id)},
                           {typeof (decimal?), (TableRow row, string id) => row.GetDecimal(id)},
                           {typeof (bool), (TableRow row, string id) => row.GetBoolean(id)},
                           {typeof (bool?), (TableRow row, string id) => row.GetBoolean(id)},
                           {typeof (DateTime), (TableRow row, string id) => row.GetDateTime(id)},
                           {typeof (DateTime?), (TableRow row, string id) => row.GetDateTime(id)},
                           {typeof (double), (TableRow row, string id) => row.GetDouble(id)},
                           {typeof (double?), (TableRow row, string id) => row.GetDouble(id)},
                           {typeof (Guid), (TableRow row, string id) => row.GetGuid(id)},
                           {typeof (Guid?), (TableRow row, string id) => row.GetGuid(id)},
                           {typeof (Enum), (TableRow row, string id) => row.GetEnum<T>(id)},
                           {typeof (char), (TableRow row, string id) => row.GetChar(id)},
                           {typeof (char?), (TableRow row, string id) => row.GetChar(id)}
                       };
        }

        private static void SetData<T>(Table table, T instance, TableRow row, Type type)
        {
            var handler = GetTypeHandlersForProperties<T>()[type];

            var propertiesThatNeedToBeSet = from property in GetPropertiesOfThisType<T>(type)
                                            from header in table.Header
                                            where IsPropertyMatchingToColumnName(property, header)
                                            select new { Header = header, PropertyName = property.Name };

            foreach (var property in propertiesThatNeedToBeSet)
                if (string.IsNullOrEmpty(row[property.Header]))
                    instance.SetPropertyValue(property.PropertyName, null);
                else
                    instance.SetPropertyValue(property.PropertyName, handler(row, property.Header));
        }

        private static IEnumerable<PropertyInfo> GetPropertiesOfThisType<T>(Type type)
        {
            return typeof(T).GetProperties().ToList()
                .Where(x => type.IsAssignableFrom(x.PropertyType));
        }
    }
}