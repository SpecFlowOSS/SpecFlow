using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableDecimalValueRetriever : IValueRetriever
    {
        private readonly Func<string, decimal> decimalValueRetriever = v => new DecimalValueRetriever().GetValue(v);

        public NullableDecimalValueRetriever(Func<string, decimal> decimalValueRetriever = null)
        {
            if (decimalValueRetriever != null)
                this.decimalValueRetriever = decimalValueRetriever;
        }

        public decimal? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return decimalValueRetriever(value);
        }

        public object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public IEnumerable<Type> TypesForWhichIRetrieveValues()
        {
            return new Type[]{ typeof(decimal?) };
        }
    }
}