using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist
{
    public static class SetComparisonExtensionMethods
    {
        public static void CompareToSet<T>(this Table table, IEnumerable<T> set)
        {
            var utility = new TableService(Config.Instance);
            new ComparisonTableStuff(utility).CompareToSet(table, set);
        }
    }
}