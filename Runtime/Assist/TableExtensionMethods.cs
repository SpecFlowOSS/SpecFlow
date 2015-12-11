using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist
{
    public static class TableHelperExtensionMethods
    {
        private static Service GetCurrentService()
        {
            //TODO[assistcont]: this will be eliminated once the extension methods move into TableServices
            return ((TableServices)TableServices.Current).Service;
        }

        public static T CreateInstance<T>(this Table table)
        {
            var instanceTable = TEHelpers.GetTheProperInstanceTable(table, typeof(T));
            return TEHelpers.ThisTypeHasADefaultConstructor<T>()
                       ? TEHelpers.CreateTheInstanceWithTheDefaultConstructor<T>(instanceTable, GetCurrentService())
                       : TEHelpers.CreateTheInstanceWithTheValuesFromTheTable<T>(instanceTable, GetCurrentService());
        }

        public static T CreateInstance<T>(this Table table, Func<T> methodToCreateTheInstance)
        {
            var instance = methodToCreateTheInstance();
            table.FillInstance(instance);
            return instance;
        }

        public static void FillInstance(this Table table, object instance)
        {
            var instanceTable = TEHelpers.GetTheProperInstanceTable(table, instance.GetType());
            TEHelpers.LoadInstanceWithKeyValuePairs(instanceTable, instance, GetCurrentService());
        }

        public static IEnumerable<T> CreateSet<T>(this Table table)
        {
            return TableServices.Current.CreateSet<T>(table);
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