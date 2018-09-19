using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableIntValueRetriever : NullableValueRetriever<int?>
    {
        private readonly Func<string, int> intValueRetriever = v => new IntValueRetriever().GetValue(v);

        public NullableIntValueRetriever(Func<string, int> intValueRetriever = null)
        {
            if (intValueRetriever != null)
                this.intValueRetriever = intValueRetriever;
        }

        protected override int? GetNonEmptyValue(string value)
        {
            return intValueRetriever(value);
        }
    }
}