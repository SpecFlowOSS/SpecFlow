using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableByteValueRetriever : IValueRetriever<byte?>
    {
        private readonly IValueRetriever<byte> byteValueRetriever;

        public NullableByteValueRetriever(IValueRetriever<byte> byteValueRetriever)
        {
            this.byteValueRetriever = byteValueRetriever;
        }

        public byte? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return byteValueRetriever.GetValue(value);
        }

        public bool TryGetValue(string text, out byte? result)
        {
            if (string.IsNullOrEmpty(text))
            {
                result = null;
                return true;
            }
            byte original;
            var tryResult = byteValueRetriever.TryGetValue(text, out original);
            result = original;
            return tryResult;
        }
    }
}