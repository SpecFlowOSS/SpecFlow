using System.Collections;
using TechTalk.SpecFlow;
using System.Linq;

namespace SpecFlow.Verify.SpecFlowPlugin.IntegrationTest.Steps;

[Binding]
internal class Steps
{
    private readonly FeatureContext _featureContext;
    private readonly ScenarioContext _scenarioContext;

    public Steps(ScenarioContext scenarioContext, FeatureContext featureContext)
    {
        _scenarioContext = scenarioContext;
        _featureContext = featureContext;
    }

    [BeforeScenario]
    public void SetupVerify()
    {
        VerifierSettings.DerivePathInfo(
            (sourceFile, projectDirectory, type, method) =>
            {
                string scenarioInfoTitle = _scenarioContext.ScenarioInfo.Title;

                if (_scenarioContext.ScenarioInfo.Arguments.Count > 0)
                {
                    scenarioInfoTitle += "_";

                    foreach (DictionaryEntry scenarioInfoArgument in _scenarioContext.ScenarioInfo.Arguments)
                    {
                        scenarioInfoTitle += "_" + scenarioInfoArgument.Value;
                    }
                }

                return new PathInfo(
                    Path.Combine(projectDirectory, _featureContext.FeatureInfo.FolderPath),
                    _featureContext.FeatureInfo.Title,
                    scenarioInfoTitle);
            });
    }

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
