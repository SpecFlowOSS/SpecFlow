using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.CustomValueRetrievers
{
    public class CustomDoubleValueRetriever : IValueRetriever<double>
    {
        public double GetValue(string text)
        {
            return double.Parse(text) + 100;
        }

        public bool TryGetValue(string text, out double result)
        {
            result = GetValue(text);
            return true;
        }
    }
}