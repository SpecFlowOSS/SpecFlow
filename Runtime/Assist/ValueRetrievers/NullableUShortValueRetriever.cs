using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableUShortValueRetriever : IValueRetriever
    {
        private readonly Func<string, ushort> ushortValueRetriever;

        public NullableUShortValueRetriever(Func<string, ushort> ushortValueRetriever)
        {
            this.ushortValueRetriever = ushortValueRetriever;
        }

        public ushort? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return ushortValueRetriever(value);
        }

        public object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public IEnumerable<Type> TypesForWhichIRetrieveValues()
        {
            return new Type[]{ typeof(ushort?) };
        }
    }
}