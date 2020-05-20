using System;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    public class GuidValueComparer : IValueComparer
    {
        private readonly GuidValueRetriever guidValueRetriever;

        public GuidValueComparer() : this(new GuidValueRetriever())
        {
        }

        public GuidValueComparer(GuidValueRetriever guidValueRetriever)
        {
            this.guidValueRetriever = guidValueRetriever;
        }

        public bool CanCompare(object actualValue)
        {
            return actualValue is Guid;
        }

        public bool Compare(string expectedValue, object actualValue)
        {
            try
            {
                return new Guid(expectedValue) == (Guid) actualValue;
            }
            catch
            {
                if (guidValueRetriever.IsAValidGuid(expectedValue) == false) return false;
                var guid = guidValueRetriever.GetValue(expectedValue);
                if (guid == Guid.Empty) return true;
                return guid == (Guid)actualValue;
            }
        }
    }
}