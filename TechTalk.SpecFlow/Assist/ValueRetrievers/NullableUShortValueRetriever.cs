using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableUShortValueRetriever : NullableValueRetriever<ushort?>
    {
        private readonly Func<string, ushort> ushortValueRetriever;

        public NullableUShortValueRetriever()
            : this(v => new UShortValueRetriever().GetValue(v))
        {
        }

        public NullableUShortValueRetriever(Func<string, ushort> ushortValueRetriever = null)
        {
            if (ushortValueRetriever != null)
                this.ushortValueRetriever = ushortValueRetriever;
        }

        protected override ushort? GetNonEmptyValue(string value)
        {
            return ushortValueRetriever(value);
        }
    }
}