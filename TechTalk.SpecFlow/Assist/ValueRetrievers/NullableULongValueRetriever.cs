using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableULongValueRetriever : NullableValueRetriever<ulong?>
    {
        private readonly Func<string, ulong> ulongValueRetriever;

        public NullableULongValueRetriever()
            : this(v => new ULongValueRetriever().GetValue(v))
        {
        }

        public NullableULongValueRetriever(Func<string, ulong> ulongValueRetriever = null)
        {
            if (ulongValueRetriever != null)
                this.ulongValueRetriever = ulongValueRetriever;
        }

        protected override ulong? GetNonEmptyValue(string value)
        {
            return ulongValueRetriever(value);
        }
    }
}