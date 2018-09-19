using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableBoolValueRetriever : NullableValueRetriever<bool?>
    {
        private readonly Func<string, bool> boolValueRetriever = v => new BoolValueRetriever().GetValue(v);

        public NullableBoolValueRetriever(Func<string, bool> boolValueRetriever = null)
        {
            if (boolValueRetriever != null)
                this.boolValueRetriever = boolValueRetriever;
        }

        protected override bool? GetNonEmptyValue(string value)
        {
            return boolValueRetriever(value);
        }
    }
}