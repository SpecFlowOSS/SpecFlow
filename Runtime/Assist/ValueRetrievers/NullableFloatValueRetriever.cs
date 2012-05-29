using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableFloatValueRetriever : IValueRetriever<float?>
    {
        private readonly IValueRetriever<float> FloatValueRetriever;

        public NullableFloatValueRetriever(IValueRetriever<float> FloatValueRetriever)
        {
            this.FloatValueRetriever = FloatValueRetriever;
        }

        public float? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return FloatValueRetriever.GetValue(value);
        }
    }
}