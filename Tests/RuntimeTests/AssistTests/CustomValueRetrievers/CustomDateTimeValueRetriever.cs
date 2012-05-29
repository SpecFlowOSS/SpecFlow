using System;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.CustomValueRetrievers
{
    public class CustomDateTimeValueRetriever : IValueRetriever<DateTime>
    {
        public DateTime GetValue(string text)
        {
            var returnValue = DateTime.MinValue;
            if (DateTime.TryParse(text, out returnValue))
                returnValue = returnValue.AddYears(10);
            return returnValue;
        }

        public bool TryGetValue(string text, out DateTime result)
        {
            if (DateTime.TryParse(text, out result))
            {
                result = result.AddYears(10);
                return true;
            }
            result = default(DateTime);
            return false;
        }
    }
}