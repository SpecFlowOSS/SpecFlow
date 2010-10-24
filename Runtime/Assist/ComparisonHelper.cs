using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist
{
    public static class ComparisonHelper
    {
        public static void CompareToInstance<T>(this Table table, T instance)
        {
            var list = new List<Difference>();

            foreach (var row in table.Rows)
            {
                var value = instance.GetPropertyValue(row["Field"]);
                if (value != row["Value"])
                {
                    list.Add(new Difference
                                 {
                                     Property = row["Field"],
                                     Expected = row["Value"],
                                     Actual = instance.GetPropertyValue(row["Field"])
                                 });
                }
            }

            if (list.Count > 0)
            {
                var aggregate = list.Aggregate(@"The following fields did not match:",
                                               (sum, next) => sum + ("\r\n" +
                                                                     string.Format("{0}: Expected <{1}>, Actual <{2}>",
                                                                                   next.Property, next.Expected,
                                                                                   next.Actual)));
                throw new ComparisonException(
                    aggregate);
            }
        }

        private class Difference
        {
            public string Property { get; set; }
            public object Expected { get; set; }
            public object Actual { get; set; }
        }
    }

    public class ComparisonException : Exception
    {
        public ComparisonException(string message)
            : base(message)
        {
        }
    }
}