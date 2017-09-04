using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist
{
    public static class SetComparisonExtensionMethods
    {
        public static void CompareToSet<T>(this Table table, IEnumerable<T> set, bool sequentialEquality = false)
        {
            var checker = new SetComparer<T>(table);
            checker.CompareToSet(set, sequentialEquality);
        }
    }
}