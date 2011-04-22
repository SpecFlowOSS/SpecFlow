using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableGuidValueRetriever
    {
        private readonly GuidValueRetriever guidValueRetriever;

        public NullableGuidValueRetriever(GuidValueRetriever guidValueRetriever)
        {
            this.guidValueRetriever = guidValueRetriever;
        }

        public Guid? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return guidValueRetriever.GetValue(value);
        }
    }
}