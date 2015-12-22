using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist
{
    public static class TableHelperExtensionMethods
    {
        public static T CreateInstance<T>(this Table table)
        {
            var tableService = new TableService(Config.Instance);
            return new TableCreationLogic(Config.Instance, tableService).CreateInstance<T>(table);
        }

        public static T CreateInstance<T>(this Table table, Func<T> methodToCreateTheInstance)
        {
            var tableService = new TableService(Config.Instance);
            return new TableCreationLogic(Config.Instance, tableService).CreateInstance<T>(table, methodToCreateTheInstance);
        }

        public static void FillInstance(this Table table, object instance)
        {
            var tableService = new TableService(Config.Instance);
            new TableCreationLogic(Config.Instance, tableService).FillInstance(table, instance);
        }

        public static IEnumerable<T> CreateSet<T>(this Table table)
        {
            var tableService = new TableService(Config.Instance);
            return new TableCreationLogic(Config.Instance, tableService).CreateSet<T>(table);
        }

        public static IEnumerable<T> CreateSet<T>(this Table table, Func<T> methodToCreateEachInstance)
        {
            var tableService = new TableService(Config.Instance);
            return new TableCreationLogic(Config.Instance, tableService).CreateSet(table, methodToCreateEachInstance);
        }
    }
}