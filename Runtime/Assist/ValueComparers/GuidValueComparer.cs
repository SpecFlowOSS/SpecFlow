using System;
using TechTalk.SpecFlow.Assist.ValueRetrievers;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    internal class GuidValueComparer : IValueComparer
    {
        private readonly IValueRetriever<Guid> guidValueRetriever;

        public GuidValueComparer()
        {
            this.guidValueRetriever = ValueRetrieverCollection.GetValueRetriever<Guid>();
        }

        public bool CanCompare(object actualValue)
        {
            return actualValue != null && actualValue.GetType() == typeof (Guid);
        }

        public bool TheseValuesAreTheSame(string expectedValue, object actualValue)
        {
            Guid guid;
            if (!guidValueRetriever.TryGetValue(expectedValue, out guid))
                return false;
            return guid == (Guid) actualValue;
        }
    }
}