using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableBoolValueRetriever : ValueRetrieverBase
    {
        private readonly Func<string, bool> boolValueRetriever = v => new BoolValueRetriever().GetValue(v);

        public NullableBoolValueRetriever(Func<string, bool> boolValueRetriever = null)
        {
            if (boolValueRetriever != null)
                this.boolValueRetriever = boolValueRetriever;
        }

        public bool? GetValue(string thisValue)
        {
            if (string.IsNullOrEmpty(thisValue)) return null;
            return boolValueRetriever(thisValue);
        }

        public override object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public override IEnumerable<Type> TypesForWhichIRetrieveValues()
        {
            return new Type[]{ typeof(bool?) };
        }
    }
}