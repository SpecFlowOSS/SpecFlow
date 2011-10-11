using System;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    internal class GuidValueComparer : IValueComparer
    {
        private readonly GuidValueRetriever guidValueRetriever;

        public GuidValueComparer(GuidValueRetriever guidValueRetriever)
        {
            this.guidValueRetriever = guidValueRetriever;
        }

        public bool CanCompare(object actualValue)
        {
            return actualValue != null && actualValue.GetType() == typeof (Guid);
        }

        public bool TheseValuesAreTheSame(string expectedValue, object actualValue)
        {
            try
            {
                return new Guid(expectedValue) == (Guid) actualValue;
            }
            catch
            {
                if (guidValueRetriever.IsAValidGuid(expectedValue) == false) return false;
                var guid = guidValueRetriever.GetValue(expectedValue);
                if (guid == new Guid()) return true;
                return guid == (Guid)actualValue;
            }
        }
    }
}