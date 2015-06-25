using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableDoubleValueRetriever : ValueRetrieverBase
    {
        private readonly Func<string, double> DoubleValueRetriever = v => new DoubleValueRetriever().GetValue(v);

        public NullableDoubleValueRetriever(Func<string, double> DoubleValueRetriever = null)
        {
            if (DoubleValueRetriever != null)
                this.DoubleValueRetriever = DoubleValueRetriever;
        }

        public double? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return DoubleValueRetriever(value);
        }

        public override object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public override IEnumerable<Type> TypesForWhichIRetrieveValues()
        {
            return new Type[]{ typeof(double?) };
        }
    }
}