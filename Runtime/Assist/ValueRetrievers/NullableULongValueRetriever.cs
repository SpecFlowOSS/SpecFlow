using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableULongValueRetriever : ValueRetrieverBase
    {
        private readonly Func<string, ulong> ulongValueRetriever = v => new ULongValueRetriever().GetValue(v);

        public NullableULongValueRetriever(Func<string, ulong> ulongValueRetriever = null)
        {
            if (ulongValueRetriever != null)
                this.ulongValueRetriever = ulongValueRetriever;
        }

        public ulong? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return ulongValueRetriever(value);
        }

        public override object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public override IEnumerable<Type> TypesForWhichIRetrieveValues()
        {
            return new Type[]{ typeof(ulong?) };
        }
    }
}