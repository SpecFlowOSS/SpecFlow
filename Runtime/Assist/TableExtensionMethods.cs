using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist
{
    public static class TableHelperExtensionMethods
    {
        public static T CreateInstance<T>(this Table table)
        {
            return new TableStuff().CreateInstance<T>(table);
        }

        public static T CreateInstance<T>(this Table table, Func<T> methodToCreateTheInstance)
        {
            return new TableStuff().CreateInstance<T>(table);
        }

        public static void FillInstance(this Table table, object instance)
        {
            new TableStuff().FillInstance(table, instance);
        }

        public static IEnumerable<T> CreateSet<T>(this Table table)
        {
            return new TableStuff().CreateSet<T>(table);
        }

        public static IEnumerable<T> CreateSet<T>(this Table table, Func<T> methodToCreateEachInstance)
        {
            return new TableStuff().CreateSet(table, methodToCreateEachInstance);
        }
    }
}