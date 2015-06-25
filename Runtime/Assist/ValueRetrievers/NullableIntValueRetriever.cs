using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableIntValueRetriever : ValueRetrieverBase
    {
        private readonly Func<string, int> intValueRetriever = v => new IntValueRetriever().GetValue(v);

        public NullableIntValueRetriever(Func<string, int> intValueRetriever = null)
        {
            if (intValueRetriever != null)
                this.intValueRetriever = intValueRetriever;
        }

        public int? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return intValueRetriever(value);
        }

        public override object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public override IEnumerable<Type> TypesForWhichIRetrieveValues()
        {
            return new Type[]{ typeof(int?) };
        }
    }
}