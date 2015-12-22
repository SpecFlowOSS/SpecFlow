using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist
{
    public interface ITableComparisonLogic
    {
        void CompareToInstance<T>(Table table, T instance);
        void CompareToSet<T>(Table table, IEnumerable<T> set);
    }
}