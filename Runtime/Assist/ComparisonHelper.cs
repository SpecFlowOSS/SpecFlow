using System;
using System.Linq;

namespace TechTalk.SpecFlow.Assist
{
    public static class ComparisonHelper
    {
        public static void CompareToInstance<T>(this Table table, T instance)
        {
            var row = table.Rows.First();
            var value = instance.GetPropertyValue(row["Field"]);
            if (value != row["Value"])
                throw new ComparisonException();
        }
    }

    public class ComparisonException : Exception
    {
    }
}