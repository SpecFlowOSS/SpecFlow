namespace TechTalk.SpecFlow.Assist
{
    public interface IValueComparer
    {
        bool CanCompare(object actualValue);
        bool TheseValuesAreTheSame(string expectedValue, object actualValue);
    }
}