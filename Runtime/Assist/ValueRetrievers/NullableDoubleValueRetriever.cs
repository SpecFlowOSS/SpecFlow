using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class NullableDoubleValueRetriever
    {
        private readonly Func<string, double> DoubleValueRetriever;

        public NullableDoubleValueRetriever(Func<string, double> DoubleValueRetriever)
        {
            this.DoubleValueRetriever = DoubleValueRetriever;
        }

        public double? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return DoubleValueRetriever(value);
        }
    }
}