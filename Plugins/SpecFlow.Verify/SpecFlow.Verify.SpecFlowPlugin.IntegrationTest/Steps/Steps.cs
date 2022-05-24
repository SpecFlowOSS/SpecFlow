using System.Threading.Tasks;
using TechTalk.SpecFlow;
using VerifyXunit;

namespace SpecFlow.Verify.SpecFlowPlugin.IntegrationTest.Steps
{
    [Binding]
    internal class Steps
    {
        [When("I try Verify with SpecFlow")]
        public async Task ITryVerifyWithSpecFlow()
        {
            await Verifier.Verify("value");
        }

        [Then("it works")]
        public void ItWorks()
        {
            
        }
    }
}
