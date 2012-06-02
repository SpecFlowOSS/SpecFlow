using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableUIntValueRetriever : IValueRetriever<uint?>
    {
        private readonly IValueRetriever<uint> uintValueRetriever;

        public NullableUIntValueRetriever(IValueRetriever<uint> uintValueRetriever)
        {
            this.uintValueRetriever = uintValueRetriever;
        }

        public uint? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return uintValueRetriever.GetValue(value);
        }

        public bool TryGetValue(string text, out uint? result)
        {
            if (string.IsNullOrEmpty(text))
            {
                result = null;
                return true;
            }
            uint original;
            var tryResult = uintValueRetriever.TryGetValue(text, out original);
            result = original;
            return tryResult;
        }
    }
}