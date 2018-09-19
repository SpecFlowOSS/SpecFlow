using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableDoubleValueRetriever : NullableValueRetriever<double?>
    {
        private readonly Func<string, double> DoubleValueRetriever;

        public NullableDoubleValueRetriever()
            : this(v => new DoubleValueRetriever().GetValue(v))
        {
        }

        public NullableDoubleValueRetriever(Func<string, double> DoubleValueRetriever = null)
        {
            if (DoubleValueRetriever != null)
                this.DoubleValueRetriever = DoubleValueRetriever;
        }

        protected override double? GetNonEmptyValue(string value)
        {
            return DoubleValueRetriever(value);
        }
    }
}