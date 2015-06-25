using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableCharValueRetriever : ValueRetrieverBase
    {
        private readonly Func<string, char> charValueRetriever = v => new CharValueRetriever().GetValue(v);

        public NullableCharValueRetriever(Func<string, char> charValueRetriever = null)
        {
            if (charValueRetriever != null)
                this.charValueRetriever = charValueRetriever;
        }

        public char? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            return charValueRetriever(value);
        }

        public override object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public override IEnumerable<Type> TypesForWhichIRetrieveValues()
        {
            return new Type[]{ typeof(char?) };
        }
    }
}