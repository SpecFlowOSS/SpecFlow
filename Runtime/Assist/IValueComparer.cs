namespace TechTalk.SpecFlow.Assist
{
    internal interface IValueComparer
    {
        bool CanCompare(object actualValue);
        bool TheseValuesAreTheSame(string expectedValue, object actualValue);
    }
}