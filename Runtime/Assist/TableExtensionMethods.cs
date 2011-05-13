using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist
{
    public static class TableHelperExtensionMethods
    {
        public static T CreateInstance<T>(this Table table)
        {
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