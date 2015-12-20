using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist
{
    public class TableStuff
    {
        public T CreateInstance<T>(Table table)
        {
            var instanceTable = TEHelpers.GetTheProperInstanceTable(table, typeof (T));
            return TEHelpers.ThisTypeHasADefaultConstructor<T>()
                ? TEHelpers.CreateTheInstanceWithTheDefaultConstructor<T>(instanceTable)
                : TEHelpers.CreateTheInstanceWithTheValuesFromTheTable<T>(instanceTable);
        }

        public T CreateInstance<T>(Table table, Func<T> methodToCreateTheInstance)
        {
            var instance = methodToCreateTheInstance();
            table.FillInstance(instance);
            return instance;
        }

        public void FillInstance(Table table, object instance)
        {
            var instanceTable = TEHelpers.GetTheProperInstanceTable(table, instance.GetType());
            TEHelpers.LoadInstanceWithKeyValuePairs(instanceTable, instance);
        }

        public IEnumerable<T> CreateSet<T>(Table table)
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

        public IEnumerable<T> CreateSet<T>(Table table, Func<T> methodToCreateEachInstance)
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

        public void CompareToSet<T>(Table table, IEnumerable<T> set)
        {
            var checker = new SetComparer<T>(table);
            checker.CompareToSet(set);
        }
    }
}