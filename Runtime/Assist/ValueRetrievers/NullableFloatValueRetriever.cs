using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class NullableFloatValueRetriever
    {
        private readonly Func<string, float> FloatValueRetriever;

        public NullableFloatValueRetriever(Func<string, float> FloatValueRetriever)
        {
            this.FloatValueRetriever = FloatValueRetriever;
        }

        public float? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return FloatValueRetriever(value);
        }
    }
}