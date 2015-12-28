using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist
{
    public interface ITableService
    {
        void CompareToSet<T>(Table table, IEnumerable<T> set);
        void CompareToInstance<T>(Table table, T instance);
        T CreateInstance<T>(Table table);
        IEnumerable<T> CreateSet<T>(Table table, Func<T> methodToCreateEachInstance);
        IEnumerable<T> CreateSet<T>(Table table);
        void FillInstance<T>(Table table, T instance);
    }
}