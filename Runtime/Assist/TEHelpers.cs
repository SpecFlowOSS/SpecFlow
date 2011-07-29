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
            var instance = (T) Activator.CreateInstance(typeof (T));
            LoadInstanceWithKeyValuePairs(table, instance);
            return instance;
        }

        internal static T CreateTheInstanceWithTheValuesFromTheTable<T>(Table table)
        {
            var constructor = GetConstructorMatchingToColumnNames<T>(table);
            if (constructor == null)
                throw new MissingMethodException(string.Format("Unable to find a suitable constructor to create instance of {0}", typeof (T).Name));

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
                    parameterValues[parameterIndex] = property.Handler(property.Row);
            }
            return (T) constructor.Invoke(parameterValues);
        }

        internal static bool ThisTypeHasADefaultConstructor<T>()
        {
            return typeof (T).GetConstructors()
                       .Where(c => c.GetParameters().Length == 0)
                       .Count() > 0;
        }

        internal static ConstructorInfo GetConstructorMatchingToColumnNames<T>(Table table)
        {
            var projectedPropertyNames = from property in typeof (T).GetProperties()
                                         from row in table.Rows
                                         where IsPropertyMatchingToColumnName(property, row.Id())
                                         select property.Name;

            return (from constructor in typeof (T).GetConstructors()
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
                .ForEach(x => instance.SetPropertyValue(x.PropertyName, x.Handler(x.Row)));
        }

        internal static IEnumerable<PropertyHandler> GetPropertiesThatNeedToBeSet<T>(Table table)
        {
            var handlers = GetTypeHandlersForFieldValuePairs<T>();

            return from property in typeof (T).GetProperties()
                   from key in handlers.Keys
                   from row in table.Rows
                   where key.IsAssignableFrom(property.PropertyType)
                         && IsPropertyMatchingToColumnName(property, row.Id())
                   select new PropertyHandler {Row = row, PropertyName = property.Name, Handler = handlers[key]};
        }

        internal static Dictionary<Type, Func<TableRow, object>> GetTypeHandlersForFieldValuePairs<T>()
        {
            return new Dictionary<Type, Func<TableRow, object>>
                       {
                           {typeof (string), (TableRow row) => new StringValueRetriever().GetValue(row[1])},
                           {typeof (int), (TableRow row) => new IntValueRetriever().GetValue(row[1])},
                           {typeof (int?), (TableRow row) => new NullableIntValueRetriever(v => new IntValueRetriever().GetValue(v)).GetValue(row[1])},
                           {typeof (decimal), (TableRow row) => new DecimalValueRetriever().GetValue(row[1])},
                           {
                               typeof (decimal?),
                               (TableRow row) => new NullableDecimalValueRetriever(v => new DecimalValueRetriever().GetValue(v)).GetValue(row[1])
                               },
                           {typeof (bool), (TableRow row) => new BoolValueRetriever().GetValue(row[1])},
                           {
                               typeof (bool?),
                               (TableRow row) => new NullableBoolValueRetriever(v => new BoolValueRetriever().GetValue(v)).GetValue(row[1])
                               },
                           {typeof (DateTime), (TableRow row) => new DateTimeValueRetriever().GetValue(row[1])},
                           {
                               typeof (DateTime?),
                               (TableRow row) => new NullableDateTimeValueRetriever(v => new DateTimeValueRetriever().GetValue(v)).GetValue(row[1])
                               },
                           {typeof (Double), (TableRow row) => new DoubleValueRetriever().GetValue(row[1])},
                           {
                               typeof (Double?),
                               (TableRow row) => new NullableDoubleValueRetriever(v => new DoubleValueRetriever().GetValue(v)).GetValue(row[1])
                               },
                           {typeof (Guid), (TableRow row) => new GuidValueRetriever().GetValue(row[1])},
                           {
                               typeof (Guid?),
                               (TableRow row) => new NullableGuidValueRetriever(v => new GuidValueRetriever().GetValue(v)).GetValue(row[1])
                               },
                           {typeof (char), (TableRow row) => new CharValueRetriever().GetValue(row[1])},
                           {
                               typeof (char?),
                               (TableRow row) => new NullableCharValueRetriever(v => new CharValueRetriever().GetValue(v)).GetValue(row[1])
                               },
                           {
                               typeof (Enum),
                               (TableRow row) =>
                               new EnumValueRetriever().GetValue(row[1], typeof (T).GetProperties().First(x => x.Name == row[0]).PropertyType)
                               }
                       };
        }

        internal class PropertyHandler
        {
            public TableRow Row { get; set; }
            public string PropertyName { get; set; }
            public Func<TableRow, object> Handler { get; set; }
        }

        internal static Table GetTheProperInstanceTable<T>(Table table)
        {
            return ThisIsAVerticalTable<T>(table)
                       ? table
                       : FlipThisHorizontalTableToAVerticalTable(table);
        }

        private static Table FlipThisHorizontalTableToAVerticalTable(Table table)
        {
            return new PivotTable(table).GetInstanceTable(0);
        }

        private static bool ThisIsAVerticalTable<T>(Table table)
        {
            if (TheHeaderIsTheOldFieldValuePair(table))
                return true;
            return (table.Rows.Count() != 1) || (table.Header.Count() == 2 && TheFirstRowValueIsTheNameOfAProperty<T>(table));
        }

        private static bool TheHeaderIsTheOldFieldValuePair(Table table)
        {
            return table.Header.Count() == 2 && table.Header.First() == "Field" && table.Header.Last() == "Value";
        }

        private static bool TheFirstRowValueIsTheNameOfAProperty<T>(Table table)
        {
            var firstRowValue = table.Rows[0][table.Header.First()];
            return typeof (T).GetProperties()
                .Any(property => IsPropertyMatchingToColumnName(property, firstRowValue));
        }
    }
}