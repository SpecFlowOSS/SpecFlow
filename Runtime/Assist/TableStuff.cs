using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist
{
    public class TableStuff
    {
        private readonly Config config;

        public TableStuff(Config config)
        {
            this.config = config;
        }

        public T CreateInstance<T>(Table table)
        {
            var helpers = new TableService(config);
            var instanceTable = helpers.GetTheProperInstanceTable(table, typeof (T));
            return helpers.ThisTypeHasADefaultConstructor<T>()
                ? helpers.CreateTheInstanceWithTheDefaultConstructor<T>(instanceTable)
                : helpers.CreateTheInstanceWithTheValuesFromTheTable<T>(instanceTable);
        }

        public T CreateInstance<T>(Table table, Func<T> methodToCreateTheInstance)
        {
            var instance = methodToCreateTheInstance();
            table.FillInstance(instance);
            return instance;
        }

        public void FillInstance(Table table, object instance)
        {
            var helpers = new TableService(config);
            var instanceTable = helpers.GetTheProperInstanceTable(table, instance.GetType());
            helpers.LoadInstanceWithKeyValuePairs(instanceTable, instance);
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
    }
}