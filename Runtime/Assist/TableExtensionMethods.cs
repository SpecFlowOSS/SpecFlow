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
                           {typeof (decimal), (TableRow row) => new DecimalValueRetriever().GetValue(row["Value"])},
                           {typeof (decimal?), (TableRow row) => new NullableDecimalValueRetriever(new DecimalValueRetriever()).GetValue(row["Value"])},
                           {typeof (bool), (TableRow row) => new BoolValueRetriever().GetValue(row["Value"])},
                           {typeof (bool?), (TableRow row) => new NullableBoolValueRetriever(new BoolValueRetriever()).GetValue(row["Value"])},
                           {typeof (DateTime), (TableRow row) => new DateTimeValueRetriever().GetValue(row["Value"])},
                           {typeof (DateTime?), (TableRow row) => new NullableDateTimeValueRetriever(new DateTimeValueRetriever()).GetValue(row["Value"])},
                           {typeof (Double), (TableRow row) => new DoubleValueRetriever().GetValue(row["Value"])},
                           {typeof (Double?), (TableRow row) => new NullableDoubleValueRetriever(new DoubleValueRetriever()).GetValue(row["Value"])},
                           {typeof (Guid), (TableRow row) => new GuidValueRetriever().GetValue(row["Value"])},
                           {typeof (Guid?), (TableRow row) => new NullableGuidValueRetriever(new GuidValueRetriever()).GetValue(row["Value"])},
                           {typeof (char), (TableRow row) => new CharValueRetriever().GetValue(row["Value"])},
                           {typeof (char?), (TableRow row) => new NullableCharValueRetriever(new CharValueRetriever()).GetValue(row["Value"])},
                           {typeof (Enum), (TableRow row) => new EnumValueRetriever().GetValue(row["Value"], typeof(T).GetProperties().First(x=>x.Name == row["Field"]).PropertyType )}
                       };
        }
    }
}