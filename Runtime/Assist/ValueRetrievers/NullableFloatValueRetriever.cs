using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableFloatValueRetriever : IValueRetriever<float?>
    {
        private readonly IValueRetriever<float> floatValueRetriever;

        public NullableFloatValueRetriever(IValueRetriever<float> FloatValueRetriever)
        {
            this.floatValueRetriever = FloatValueRetriever;
        }

        public float? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return floatValueRetriever.GetValue(value);
        }

        public bool TryGetValue(string text, out float? result)
        {
            if (string.IsNullOrEmpty(text))
            {
                result = null;
                return true;
            }
            float original;
            var tryResult = floatValueRetriever.TryGetValue(text, out original);
            result = original;
            return tryResult;
        }
    }
}