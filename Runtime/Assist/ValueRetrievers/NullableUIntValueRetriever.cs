using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class NullableUIntValueRetriever
    {
        private readonly Func<string, uint> uintValueRetriever;

        public NullableUIntValueRetriever(Func<string, uint> uintValueRetriever)
        {
            this.uintValueRetriever = uintValueRetriever;
        }

        public uint? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return uintValueRetriever(value);
        }
    }
}