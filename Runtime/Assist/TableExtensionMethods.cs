using System;
using System.Collections.Generic;
using System.Linq;

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
            var enumerable = table.Rows.Select(row =>
            {
                var instance = (T)Activator.CreateInstance(typeof(T));
                LoadInstanceWithPropertyData<T>(table, instance, row);
                return instance;
            });

            return enumerable;
        }

        private static void LoadInstanceWithPropertyData<T>(Table table, T instance, TableRow row)
        {
            foreach (var key in GetTypeHandlersForProperties<T>().Keys)
                SetData(table, instance, row, key);
        }

        private static void LoadInstanceWithKeyValuePairs<T>(Table table, T instance)
        {
            var handlers = GetTypeHandlersForFieldValuePairs<T>();

            var propertiesThatNeedToBeSet = from property in typeof(T).GetProperties()
                                            from key in handlers.Keys
                                            from row in table.Rows
                                            where key.IsAssignableFrom(property.PropertyType)
                                                && property.Name == row["Field"]
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
                .Where(x => type.IsAssignableFrom(x.PropertyType))
                .Select(x => x.Name);
        }
    }
}