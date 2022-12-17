using TechTalk.SpecFlow;

namespace SpecFlow.Verify.SpecFlowPlugin.IntegrationTest.Steps;

[Binding]
internal class Steps
{
    [When("I try Verify with SpecFlow")]
    public Task ITryVerifyWithSpecFlow()
    {
        return Verifier.Verify("value");
    }

    [When(@"I try Verify with SpecFlow for Parameter '([^']*)'")]
    public Task WhenITryVerifyWithSpecFlowForParameter(string p0)
    {
        return Verifier.Verify("value");
    }

    [Then("it works")]
    public void ItWorks()
    {
    }
}
