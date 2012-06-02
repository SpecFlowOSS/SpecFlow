using System;

namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    internal class DateTimeValueComparer : ValueComparerBase<DateTime>
    {
        public override bool TheseValuesAreTheSame(string expectedValue, object actualValue)
        {
            return TheseValuesAreTheSame(expectedValue, actualValue,
                (e, a) => (DateTime) e == (DateTime) a);
        }
    }
}