using System;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.CustomValueRetrievers
{
    public class CustomGuidValueRetriever : IValueRetriever<Guid>
    {
        public Guid GetValue(string text)
        {
            return new Guid("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA");
        }

        public bool TryGetValue(string text, out Guid result)
        {
            result = GetValue(text);
            return true;
        }
    }
}