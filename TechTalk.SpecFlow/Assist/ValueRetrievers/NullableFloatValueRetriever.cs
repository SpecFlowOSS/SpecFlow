using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableFloatValueRetriever : IValueRetriever
    {
        private readonly Func<string, float> FloatValueRetriever = v => new FloatValueRetriever().GetValue(v);

        public NullableFloatValueRetriever(Func<string, float> FloatValueRetriever = null)
        {
            if (FloatValueRetriever != null)
                this.FloatValueRetriever = FloatValueRetriever;
        }

        public virtual float? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return FloatValueRetriever(value);
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(float?);
        }
    }
}