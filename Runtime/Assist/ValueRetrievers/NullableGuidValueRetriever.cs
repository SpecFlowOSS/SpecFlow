using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableGuidValueRetriever : IValueRetriever<Guid?>
    {
        private readonly IValueRetriever<Guid> guidValueRetriever;

        public NullableGuidValueRetriever(IValueRetriever<Guid> guidValueRetriever)
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