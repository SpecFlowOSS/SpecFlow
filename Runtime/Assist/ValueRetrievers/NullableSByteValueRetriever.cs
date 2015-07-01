using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableSByteValueRetriever : IValueRetriever
    {
        private readonly Func<string, sbyte> sbyteValueRetriever = v => new SByteValueRetriever().GetValue(v);

        public NullableSByteValueRetriever(Func<string, sbyte> sbyteValueRetriever = null)
        {
            if (sbyteValueRetriever != null)
                this.sbyteValueRetriever = sbyteValueRetriever;
        }

        public sbyte? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return sbyteValueRetriever(value);
        }

        public object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public bool CanRetrieve(Type type)
        {
            return type == typeof(sbyte?);
        }
    }
}