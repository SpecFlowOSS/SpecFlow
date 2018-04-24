namespace TechTalk.SpecFlow.Rpc.Tests.Integration
{
    public class TestServerInterface : ITestServerInterface
    {
        public string MethodWithParameter(string parameter)
        {
            return parameter;
        }
    }
}