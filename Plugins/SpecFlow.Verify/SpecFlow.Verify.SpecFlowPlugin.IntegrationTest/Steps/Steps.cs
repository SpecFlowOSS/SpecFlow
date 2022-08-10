using TechTalk.SpecFlow;

namespace SpecFlow.Verify.SpecFlowPlugin.IntegrationTest.Steps;

[Binding]
internal class Steps
{
    [When("I try Verify with SpecFlow")]
    public async Task ITryVerifyWithSpecFlow()
    {
        await Verifier.Verify("value");
    }

    [When(@"I try Verify with SpecFlow for Parameter '([^']*)'")]
    public async Task WhenITryVerifyWithSpecFlowForParameter(string p0)
    {
        await Verifier.Verify("value");
    }

    [Then("it works")]
    public void ItWorks()
    {
    }
}
