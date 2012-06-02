using System;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    internal class GuidValueComparer : ValueComparerBase<Guid>
    {
        public override bool TheseValuesAreTheSame(string expectedValue, object actualValue)
        {
            return TheseValuesAreTheSame(expectedValue, actualValue,
                (e, a) => (Guid) e == (Guid) a);
        }
    }
}