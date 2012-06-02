using System;

namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    internal class DecimalValueComparer : ValueComparerBase<decimal>
    {
        public override bool TheseValuesAreTheSame(string expectedValue, object actualValue)
        {
            return TheseValuesAreTheSame(expectedValue, actualValue, (e, a) => (decimal)e == (decimal)a);
        }
    }
}