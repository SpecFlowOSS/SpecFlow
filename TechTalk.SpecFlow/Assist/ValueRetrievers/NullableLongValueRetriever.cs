using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableLongValueRetriever : NullableValueRetriever<long?>
    {
        private readonly Func<string, long> longValueRetriever;

        public NullableLongValueRetriever()
            : this(v => new LongValueRetriever().GetValue(v))
        {
        }

        public NullableLongValueRetriever(Func<string, long> longValueRetriever = null)
        {
            if (longValueRetriever != null)
                this.longValueRetriever = longValueRetriever;
        }

        protected override long? GetNonEmptyValue(string value)
        {
            return longValueRetriever(value);
        }
    }
}