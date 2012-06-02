using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.CustomValueRetrievers
{
    public class CustomBoolValueRetriever : IValueRetriever<bool>
    {
        public bool GetValue(string text)
        {
            return !bool.Parse(text);
        }

        public bool TryGetValue(string text, out bool result)
        {
            bool obj;
            if (bool.TryParse(text, out obj))
            {
                result = !obj;
                return true;
            }
            result = default(bool);
            return false;
        }
    }
}