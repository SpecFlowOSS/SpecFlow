using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableByteValueRetriever : NullableValueRetriever<byte?>
    {
        private readonly Func<string, byte> byteValueRetriever;

        public NullableByteValueRetriever()
            : this(v => new ByteValueRetriever().GetValue(v))
        {
        }

        public NullableByteValueRetriever(Func<string, byte> byteValueRetriever = null)
        {
            if (byteValueRetriever != null)
                this.byteValueRetriever = byteValueRetriever;
        }

        protected override byte? GetNonEmptyValue(string value)
        {
            return byteValueRetriever(value);
        }
    }
}