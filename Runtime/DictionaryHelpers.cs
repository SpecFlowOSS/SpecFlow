namespace TechTalk.SpecFlow
{
    public static class DictionaryHelpers
    {
        public static void Set<T>(this ScenarioContext scenarioContext, T data)
        {
            var id = typeof (T).ToString();
            scenarioContext[id] = data;
        }

        public static T Get<T>(this ScenarioContext scenarioContext) where T : class
        {
            var id = typeof (T).ToString();
            return scenarioContext[id] as T;
        }
    }
}