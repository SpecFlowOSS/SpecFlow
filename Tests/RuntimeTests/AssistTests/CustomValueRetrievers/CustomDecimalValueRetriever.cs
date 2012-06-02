using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.CustomValueRetrievers
{
    public class CustomDecimalValueRetriever : IValueRetriever<decimal>
    {
        public decimal GetValue(string text)
        {
            return decimal.Parse(text) + 100;
        }

        public bool TryGetValue(string text, out decimal result)
        {
            result = GetValue(text);
            return true;
        }
    }
}