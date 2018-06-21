namespace TechTalk.SpecFlow.UnitTestProvider
{
    public class UnitTestProviderConfiguration
    {
        public void UseUnitTestProvider(string unitTestProviderName) { } //throws an exception if UnitTestProvider is already set

        public string UnitTestProvider { get; }
    }
}
