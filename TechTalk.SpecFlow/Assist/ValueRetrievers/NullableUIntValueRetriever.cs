using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableUIntValueRetriever : NullableValueRetriever<uint?>
    {
        private readonly Func<string, uint> uintValueRetriever = v => new UIntValueRetriever().GetValue(v);

        public NullableUIntValueRetriever(Func<string, uint> uintValueRetriever = null)
        {
            if (uintValueRetriever != null)
                this.uintValueRetriever = uintValueRetriever;
        }

        protected override uint? GetNonEmptyValue(string value)
        {
            return uintValueRetriever(value);
        }
    }
}