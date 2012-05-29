using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableDoubleValueRetriever : IValueRetriever<double?>
    {
        private readonly IValueRetriever<double> DoubleValueRetriever;

        public NullableDoubleValueRetriever(IValueRetriever<double> DoubleValueRetriever)
        {
            this.DoubleValueRetriever = DoubleValueRetriever;
        }

        public double? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return DoubleValueRetriever.GetValue(value);
        }

        public bool TryGetValue(string text, out double? result)
        {
            if (string.IsNullOrEmpty(text))
            {
                result = null;
                return true;
            }
            double original;
            var tryResult = DoubleValueRetriever.TryGetValue(text, out original);
            result = original;
            return tryResult;
        }
    }
}