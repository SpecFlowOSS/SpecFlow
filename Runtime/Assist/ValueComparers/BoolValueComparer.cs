using System;

namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    internal class BoolValueComparer : ValueComparerBase<bool>
    {
        public override bool TheseValuesAreTheSame(string expectedValue, object actualValue)
        {
            return TheseValuesAreTheSame(expectedValue, actualValue, (e, a) => (bool) e == (bool) a);
        }
    }
}