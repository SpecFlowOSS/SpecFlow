namespace TechTalk.SpecFlow.Assist.ValueComparers
{
    public class DecimalValueComparer : IValueComparer
    {
        public bool CanCompare(object actualValue)
        {
            return actualValue is decimal;
        }

        public bool Compare(string expectedValue, object actualValue)
        {
            return decimal.TryParse(expectedValue, out var expected) &&
                   expected == (decimal)actualValue;
        }
    }
}