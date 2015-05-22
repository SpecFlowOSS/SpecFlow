using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableCharValueRetriever : IValueRetriever
    {
        private readonly Func<string, char> charValueRetriever;

        public NullableCharValueRetriever(Func<string, char> charValueRetriever)
        {
            this.charValueRetriever = charValueRetriever;
        }

        public char? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            return charValueRetriever(value);
        }

        public object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public IEnumerable<Type> TypesForWhichIRetrieveValues()
        {
            return new Type[]{ typeof(char?) };
        }
    }
}