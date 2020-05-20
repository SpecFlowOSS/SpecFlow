using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist
{
    public static class TableHelperExtensionMethods
    {
        public static T CreateInstance<T>(this Table table)
        {
            return CreateInstance<T>(table, (InstanceCreationOptions)null);
        }

        public static T CreateInstance<T>(this Table table, InstanceCreationOptions creationOptions)
        {
            var instanceTable = TEHelpers.GetTheProperInstanceTable(table, typeof(T));
            return TEHelpers.ThisTypeHasADefaultConstructor<T>()
                       ? TEHelpers.CreateTheInstanceWithTheDefaultConstructor<T>(instanceTable, creationOptions)
                       : TEHelpers.CreateTheInstanceWithTheValuesFromTheTable<T>(instanceTable, creationOptions);
        }

        public static T CreateInstance<T>(this Table table, Func<T> methodToCreateTheInstance)
        {
            return CreateInstance(table, methodToCreateTheInstance, null);
        }

        public static T CreateInstance<T>(this Table table, Func<T> methodToCreateTheInstance, InstanceCreationOptions creationOptions)
        {
            var instance = methodToCreateTheInstance();
            table.FillInstance(instance, creationOptions);
            return instance;
        }

        public static void FillInstance(this Table table, object instance)
        {
            FillInstance(table, instance, null);
        }

        public static void FillInstance(this Table table, object instance, InstanceCreationOptions creationOptions)
        {
            var instanceTable = TEHelpers.GetTheProperInstanceTable(table, instance.GetType());
            TEHelpers.LoadInstanceWithKeyValuePairs(instanceTable, instance, creationOptions);
        }

        public static IEnumerable<T> CreateSet<T>(this Table table)
        {
            return CreateSet<T>(table, (InstanceCreationOptions)null);
        }

        public static IEnumerable<T> CreateSet<T>(this Table table, InstanceCreationOptions creationOptions)
        {
            int count = table.Rows.Count;

            var list = new List<T>(count);


            var pivotTable = new PivotTable(table);
            for (var index = 0; index < count; index++)
            {
                var instance = pivotTable.GetInstanceTable(index).CreateInstance<T>(creationOptions);
                list.Add(instance);
            }

            return list;
        }

        public static IEnumerable<T> CreateSet<T>(this Table table, Func<T> methodToCreateEachInstance)
        {
            return CreateSet(table, methodToCreateEachInstance, null);
        }

        public static IEnumerable<T> CreateSet<T>(this Table table, Func<T> methodToCreateEachInstance, InstanceCreationOptions creationOptions)
        {
            int count = table.Rows.Count;
            var list = new List<T>(count);

            var pivotTable = new PivotTable(table);
            for (var index = 0; index < count; index++)
            {
                var instance = methodToCreateEachInstance();
                pivotTable.GetInstanceTable(index).FillInstance(instance, creationOptions);
                list.Add(instance);
            }

            return list;
        }

        public static IEnumerable<T> CreateSet<T>(this Table table, Func<TableRow, T> methodToCreateEachInstance)
        {
            return CreateSet(table, methodToCreateEachInstance, null);
        }

        public static IEnumerable<T> CreateSet<T>(this Table table, Func<TableRow, T> methodToCreateEachInstance, InstanceCreationOptions creationOptions)
        {
            int count = table.Rows.Count;
            var list = new List<T>(count);

            var pivotTable = new PivotTable(table);
            for (var index = 0; index < count; index++)
            {
                var row = table.Rows[index];
                var instance = methodToCreateEachInstance(row);
                pivotTable.GetInstanceTable(index).FillInstance(instance, creationOptions);
                list.Add(instance);
            }

            return list;
        }
    }
}
