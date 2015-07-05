namespace TechTalk.SpecFlow.Assist
{
    public interface IValueComparer
    {
        bool CanCompare(object actualValue);
        bool Compare(string expectedValue, object actualValue);
    }
}