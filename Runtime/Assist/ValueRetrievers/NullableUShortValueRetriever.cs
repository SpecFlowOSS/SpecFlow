using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableUShortValueRetriever
    {
        private readonly Func<string, ushort> ushortValueRetriever;

        public NullableUShortValueRetriever(Func<string, ushort> ushortValueRetriever)
        {
            this.ushortValueRetriever = ushortValueRetriever;
        }

        public ushort? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return ushortValueRetriever(value);
        }
    }
}