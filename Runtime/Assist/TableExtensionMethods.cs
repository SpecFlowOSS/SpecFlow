using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist
{
    public static class TableHelperExtensionMethods
    {
        public static T CreateInstance<T>(this Table table)
        {
            var name = table.Rows.Count() > 0 ? table.Rows[0][table.Header.First()] : null;
            if (table.Rows.Count() == 1
                && (table.Header.Count() != 2 ||
                    typeof (T).GetProperties().Any(x => TEHelpers.IsPropertyMatchingToColumnName(x, name)) == false
                   ))
            {
                var pivotTable = new PivotTable(table);
                table = pivotTable.GetInstanceTable(0);
            }
            return TEHelpers.ThisTypeHasADefaultConstructor<T>()
                       ? TEHelpers.CreateTheInstanceWithTheDefaultConstructor<T>(table)
                       : TEHelpers.CreateTheInstanceWithTheValuesFromTheTable<T>(table);
        }

        public static void FillInstance<T>(this Table table, T instance)
        {
            TEHelpers.LoadInstanceWithKeyValuePairs(table, instance);
        }

        public static IEnumerable<T> CreateSet<T>(this Table table)
        {
            var list = new List<T>();

            var pivotTable = new PivotTable(table);
            for (var index = 0; index < table.Rows.Count(); index++)
            {
                var instance = pivotTable.GetInstanceTable(index).CreateInstance<T>();
                list.Add(instance);
            }

            return list;
        }
    }
}