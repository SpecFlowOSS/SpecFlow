using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableUShortValueRetriever : IValueRetriever<ushort?>
    {
        private readonly IValueRetriever<ushort> ushortValueRetriever;

        public NullableUShortValueRetriever(IValueRetriever<ushort> ushortValueRetriever)
        {
            this.ushortValueRetriever = ushortValueRetriever;
        }

        public ushort? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return ushortValueRetriever.GetValue(value);
        }
    }
}