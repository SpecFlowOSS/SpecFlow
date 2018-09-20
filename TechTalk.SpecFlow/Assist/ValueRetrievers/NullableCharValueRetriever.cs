using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableCharValueRetriever : NullableValueRetriever<char?>
    {
        private readonly Func<string, char> charValueRetriever;

        public NullableCharValueRetriever()
            : this(v => new CharValueRetriever().GetValue(v))
        {
        }

        public NullableCharValueRetriever(Func<string, char> charValueRetriever = null)
        {
            if (charValueRetriever != null)
                this.charValueRetriever = charValueRetriever;
        }

        protected override char? GetNonEmptyValue(string value)
        {
            return charValueRetriever(value);
        }
    }
}