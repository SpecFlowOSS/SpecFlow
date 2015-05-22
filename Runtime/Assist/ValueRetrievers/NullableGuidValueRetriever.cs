using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableGuidValueRetriever : IValueRetriever
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