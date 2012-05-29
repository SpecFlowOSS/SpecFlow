using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class NullableBoolValueRetriever : IValueRetriever<bool?>
    {
        private readonly IValueRetriever<bool> boolValueRetriever;

        public NullableBoolValueRetriever(IValueRetriever<bool> boolValueRetriever)
        {
            this.boolValueRetriever = boolValueRetriever;
        }

        public bool? GetValue(string thisValue)
        {
            if (string.IsNullOrEmpty(thisValue)) return null;
            return boolValueRetriever.GetValue(thisValue);
        }
    }
}