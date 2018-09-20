using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableFloatValueRetriever : NullableValueRetriever<float?>
    {
        private readonly Func<string, float> FloatValueRetriever;

        public NullableFloatValueRetriever()
            : this(v => new FloatValueRetriever().GetValue(v))
        {
        }

        public NullableFloatValueRetriever(Func<string, float> FloatValueRetriever = null)
        {
            if (FloatValueRetriever != null)
                this.FloatValueRetriever = FloatValueRetriever;
        }

        protected override float? GetNonEmptyValue(string value)
        {
            return FloatValueRetriever(value);
        }
    }
}