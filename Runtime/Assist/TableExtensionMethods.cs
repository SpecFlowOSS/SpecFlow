using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist
{
    public static class TableHelperExtensionMethods
    {
        public static T CreateInstance<T>(this Table table)
        {
            var instanceTable = TEHelpers.GetTheProperInstanceTable<T>(table);
            return TEHelpers.ThisTypeHasADefaultConstructor<T>()
                       ? TEHelpers.CreateTheInstanceWithTheDefaultConstructor<T>(instanceTable)
                       : TEHelpers.CreateTheInstanceWithTheValuesFromTheTable<T>(instanceTable);
        }

        public static T CreateInstance<T>(this Table table, Func<T> methodToCreateTheInstance)
        {
            var instance = methodToCreateTheInstance();
            table.FillInstance(instance);
            return instance;
        }

        public static void FillInstance<T>(this Table table, T instance)
        {
            var instanceTable = TEHelpers.GetTheProperInstanceTable<T>(table);
            TEHelpers.LoadInstanceWithKeyValuePairs(instanceTable, instance);
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

        public static IEnumerable<T> CreateSet<T>(this Table table, Func<T> methodToCreateEachInstance)
        {
            var list = new List<T>();

            var pivotTable = new PivotTable(table);
            for (var index = 0; index < table.Rows.Count(); index++)
            {
                var instance = methodToCreateEachInstance();
                pivotTable.GetInstanceTable(index).FillInstance(instance);
                list.Add(instance);
            }

            return list;
        }
    }
}