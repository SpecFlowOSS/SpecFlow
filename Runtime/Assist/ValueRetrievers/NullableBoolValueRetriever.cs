using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    internal class NullableBoolValueRetriever
    {
        private readonly Func<string, bool> boolValueRetriever;

        public NullableBoolValueRetriever(Func<string, bool> boolValueRetriever)
        {
            this.boolValueRetriever = boolValueRetriever;
        }

        public bool? GetValue(string thisValue)
        {
            if (string.IsNullOrEmpty(thisValue)) return null;
            return boolValueRetriever(thisValue);
        }
    }
}