using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class NullableGuidValueRetriever
    {
        private readonly Func<string, Guid> guidValueRetriever;

        public NullableGuidValueRetriever(Func<string, Guid> guidValueRetriever)
        {
            this.guidValueRetriever = guidValueRetriever;
        }

        public Guid? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return guidValueRetriever(value);
        }
    }
}