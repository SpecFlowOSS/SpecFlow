using System;

namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    internal class DoubleValueComparer : ValueComparerBase<double>
    {
        public override bool TheseValuesAreTheSame(string expectedValue, object actualValue)
        {
            return TheseValuesAreTheSame(expectedValue, actualValue, (e, a) => (double) e == (double) a);
        }
    }
}