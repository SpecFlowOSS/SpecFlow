using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableSByteValueRetriever : NullableValueRetriever<sbyte?>
    {
        private readonly Func<string, sbyte> sbyteValueRetriever;

        public NullableSByteValueRetriever()
            : this(v => new SByteValueRetriever().GetValue(v))
        {
        }

        public NullableSByteValueRetriever(Func<string, sbyte> sbyteValueRetriever = null)
        {
            if (sbyteValueRetriever != null)
                this.sbyteValueRetriever = sbyteValueRetriever;
        }

        protected override sbyte? GetNonEmptyValue(string value)
        {
            return sbyteValueRetriever(value);
        }
    }
}