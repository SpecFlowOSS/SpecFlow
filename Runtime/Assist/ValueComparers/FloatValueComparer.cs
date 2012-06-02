using System;

namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    internal class FloatValueComparer : ValueComparerBase<float>
    {
        public override bool TheseValuesAreTheSame(string expectedValue, object actualValue)
        {
            return TheseValuesAreTheSame(expectedValue, actualValue, (e, a) => (float)e == (float)a);
        }
    }
}