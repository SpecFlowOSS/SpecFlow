using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableFloatValueRetriever : ValueRetrieverBase
    {
        private readonly Func<string, float> FloatValueRetriever = v => new FloatValueRetriever().GetValue(v);

        public NullableFloatValueRetriever(Func<string, float> FloatValueRetriever = null)
        {
            if (FloatValueRetriever != null)
                this.FloatValueRetriever = FloatValueRetriever;
        }

        public float? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return FloatValueRetriever(value);
        }

        public override object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }

        public override bool CanRetrieve(Type type)
        {
            return type == typeof(float?);
        }
    }
}