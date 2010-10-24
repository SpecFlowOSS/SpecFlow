using System;

namespace TechTalk.SpecFlow.Assist
{
    public static class ComparisonHelper
    {
        public static void CompareToInstance<T>(this Table table, T instance)
        {
            foreach (var row in table.Rows)
            {
                var value = instance.GetPropertyValue(row["Field"]);
                if (value != row["Value"])
                    throw new ComparisonException();
            }
        }
    }

    public class ComparisonException : Exception
    {
    }
}