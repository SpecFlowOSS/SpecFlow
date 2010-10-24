using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow.Assist
{
    public static class SetComparisonExtensionMethods
    {
        public static void CompareToSet<T>(this Table table, IEnumerable<T> set)
        {
            if (set.Count() > 0 || table.Rows.Count() > 0) 
                throw new ComparisonException("");
        }
    }
}
