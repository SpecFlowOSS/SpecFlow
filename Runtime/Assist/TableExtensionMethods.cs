using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist
{
    public static class TableExtensionMethods
    {
        public static T CreateInstance<T>(this Table table)
        {
            var instance = (T)Activator.CreateInstance(typeof(T));
            LoadInstanceWithKeyValuePairs(table, instance);
            return instance;
        }

        public static IEnumerable<T> CreateSet<T>(this Table table)
        {
            var enumerable = table.Rows.Select(row =>
            {
                var instance = (T)Activator.CreateInstance(typeof(T));
                LoadInstanceWithPropertyData(table, instance, row);
                return instance;
            });

            return enumerable;
        }

        private static void LoadInstanceWithPropertyData<T>(Table table, T instance, TableRow row)
        {
            foreach (var key in GetTypeHandlersForProperties().Keys)
                SetData(table, instance, row, key);
        }

        private static void LoadInstanceWithKeyValuePairs<T>(Table table, T instance)
        {
            var handlers = GetTypeHandlersForFieldValuePairs();

            var propertiesThatNeedToBeSet = (from property in typeof(T).GetProperties()
                                             join key in handlers.Keys on property.PropertyType equals key
                                             join row in table.Rows on property.Name equals row["Field"]
                                             select new { Row = row, property.Name, Handler = handlers[key] });

            propertiesThatNeedToBeSet.ToList()
                .ForEach(x => instance.SetPropertyValue(x.Name, x.Handler(x.Row, x.Row["Value"])));
        }

        private static Dictionary<Type, Func<TableRow, string, object>> GetTypeHandlersForFieldValuePairs()
        {
            return new Dictionary<Type, Func<TableRow, string, object>>
                       {
                           {typeof (string), (TableRow row, string id) => row.GetString("Value")},
                           {typeof (int), (TableRow row, string id) => row.GetInt32("Value")},
                           {typeof (decimal), (TableRow row, string id) => row.GetDecimal("Value")},
                           {typeof (bool), (TableRow row, string id) => row.GetBoolean("Value")},
                           {typeof (bool?), (TableRow row, string id) => row.GetBoolean("Value")},
                           {typeof (DateTime), (TableRow row, string id) => row.GetDateTime("Value")},
                           {typeof (DateTime?), (TableRow row, string id) => row.GetDateTime("Value")}
                       };
        }

        private static Dictionary<Type, Func<TableRow, string, object>> GetTypeHandlersForProperties()
        {
            return new Dictionary<Type, Func<TableRow, string, object>>
                       {
                           {typeof (string), (TableRow row, string id) => row.GetString(id)},
                           {typeof (int), (TableRow row, string id) => row.GetInt32(id)},
                           {typeof (decimal), (TableRow row, string id) => row.GetDecimal(id)},
                           {typeof (bool), (TableRow row, string id) => row.GetBoolean(id)},
                           {typeof (DateTime), (TableRow row, string id) => row.GetDateTime(id)},
                           {typeof (int?), (TableRow row, string id) => row.GetInt32(id)},
                           {typeof (decimal?), (TableRow row, string id) => row.GetDecimal(id)},
                           {typeof (bool?), (TableRow row, string id) => row.GetBoolean(id)},
                           {typeof (DateTime?), (TableRow row, string id) => row.GetDateTime(id)}
                       };
        }

        private static void SetData<T>(Table table, T instance, TableRow row, Type type)
        {
            var handler = GetTypeHandlersForProperties()[type];
            var propertiesToUpdate = GetPropertiesOfThisTypeToUpdate<T>(table, type);
            foreach (var property in propertiesToUpdate)
                if (string.IsNullOrEmpty(row[property]))
                    instance.SetPropertyValue(property, null);
                else
                    instance.SetPropertyValue(property, handler(row, property));
        }

        private static IEnumerable<string> GetPropertiesOfThisTypeToUpdate<T>(Table table, Type type)
        {
            var propertyNames = GetPropertiesOfThisType<T>(type);

            return from name in propertyNames
                   join header in table.Header on name equals header
                   select header;
        }

        private static IEnumerable<string> GetPropertiesOfThisType<T>(Type type)
        {
            return typeof(T).GetProperties().ToList()
                .Where(x => x.PropertyType == type)
                .Select(x => x.Name);
        }
    }
}
