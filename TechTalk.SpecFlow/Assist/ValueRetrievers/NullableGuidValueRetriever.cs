using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableGuidValueRetriever : NullableValueRetriever<Guid?>
    {
        private readonly Func<string, Guid> guidValueRetriever = v => new GuidValueRetriever().GetValue(v);

        public NullableGuidValueRetriever(Func<string, Guid> guidValueRetriever = null)
        {
            if (guidValueRetriever != null)
                this.guidValueRetriever = guidValueRetriever;
        }

        protected override Guid? GetNonEmptyValue(string value)
        {
            return guidValueRetriever(value);
        }
    }
}