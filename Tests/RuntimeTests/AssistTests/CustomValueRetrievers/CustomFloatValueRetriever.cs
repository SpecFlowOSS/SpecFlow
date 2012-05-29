using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.CustomValueRetrievers
{
    public class CustomFloatValueRetriever : IValueRetriever<float>
    {
        public float GetValue(string text)
        {
            return float.Parse(text) + 100;
        }

        public bool TryGetValue(string text, out float result)
        {
            result = GetValue(text);
            return true;
        }
    }
}