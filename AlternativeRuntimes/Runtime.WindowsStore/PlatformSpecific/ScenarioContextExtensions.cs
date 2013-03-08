namespace TechTalk.SpecFlow
{
    public static class ScenarioContextExtensions
    {
        private const string TestInstanceKey = "__TestInstance__";

        public static void SetTestInstance(this ScenarioContext context, object testInstance)
        {
            context.Set(testInstance, TestInstanceKey);
        }

        public static T GetTestInstance<T>(this ScenarioContext context)
        {
            return (T)context.Get<object>(TestInstanceKey);
        }
    }
}