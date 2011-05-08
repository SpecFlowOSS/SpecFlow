namespace TechTalk.SpecFlow.Assist
{
    internal interface IValueComparer
    {
        bool CanCompare(object actualValue);
        bool CompareValue(string expectedValue, object actualValue);
    }
}