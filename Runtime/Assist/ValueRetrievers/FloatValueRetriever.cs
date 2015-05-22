using System;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class FloatValueRetriever : IValueRetriever
    {
        public virtual float GetValue(string value)
        {
            float returnValue = 0F;
            float.TryParse(value, out returnValue);
            return returnValue;
        }

        public object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return GetValue(row[1]);
        }
    }
}