using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableShortValueRetriever : NullableValueRetriever<short?>
    {
        private readonly Func<string, short> shortValueRetriever = v => new ShortValueRetriever().GetValue(v);

        public NullableShortValueRetriever(Func<string, short> shortValueRetriever = null)
        {
            if (shortValueRetriever != null)
                this.shortValueRetriever = shortValueRetriever;
        }

        protected override short? GetNonEmptyValue(string value)
        {
            return shortValueRetriever(value);
        }
    }
}