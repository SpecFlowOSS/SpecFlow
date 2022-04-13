using System;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.Infrastructure;

public class BlockingSyncTestRunner : ISyncTestRunner
{
    private readonly ITestRunner _testRunner;

    public BlockingSyncTestRunner(ITestRunner testRunner)
    {
        _testRunner = testRunner;
    }
    private void SyncWrapper(Func<Task> asyncCall)
    {
        asyncCall().GetAwaiter().GetResult();
    }

    public string TestWorkerId => _testRunner.TestWorkerId;
    public FeatureContext FeatureContext => _testRunner.FeatureContext;
    public ScenarioContext ScenarioContext => _testRunner.ScenarioContext;

    public void OnScenarioInitialize(ScenarioInfo scenarioInfo)
    {
        _testRunner.OnScenarioInitialize(scenarioInfo);
    }

    public void SkipScenario()
    {
        _testRunner.SkipScenario();
    }

    public void Pending()
    {
        _testRunner.Pending();
    }

    public void OnTestRunStart()
    {
        SyncWrapper(() => _testRunner.OnTestRunStartAsync());
    }

    public void OnTestRunEnd()
    {
        SyncWrapper(() => _testRunner.OnTestRunEndAsync());
    }

    public void OnFeatureStart(FeatureInfo featureInfo)
    {
        SyncWrapper(() => _testRunner.OnFeatureStartAsync(featureInfo));
    }

    public void OnFeatureEnd()
    {
        SyncWrapper(() => _testRunner.OnFeatureEndAsync());
    }

    public void OnScenarioStart()
    {
        SyncWrapper(() => _testRunner.OnScenarioStartAsync());
    }

    public void CollectScenarioErrors()
    {
        SyncWrapper(() => _testRunner.CollectScenarioErrorsAsync());
    }

    public void OnScenarioEnd()
    {
        SyncWrapper(() => _testRunner.OnScenarioEndAsync());
    }

    public void Given(string text, string multilineTextArg, Table tableArg, string keyword = null)
    {
        SyncWrapper(() => _testRunner.GivenAsync(text, multilineTextArg, tableArg, keyword));
    }

    public void When(string text, string multilineTextArg, Table tableArg, string keyword = null)
    {
        SyncWrapper(() => _testRunner.WhenAsync(text, multilineTextArg, tableArg, keyword));
    }

    public void Then(string text, string multilineTextArg, Table tableArg, string keyword = null)
    {
        SyncWrapper(() => _testRunner.ThenAsync(text, multilineTextArg, tableArg, keyword));
    }

    public void And(string text, string multilineTextArg, Table tableArg, string keyword = null)
    {
        SyncWrapper(() => _testRunner.AndAsync(text, multilineTextArg, tableArg, keyword));
    }

    public void But(string text, string multilineTextArg, Table tableArg, string keyword = null)
    {
        SyncWrapper(() => _testRunner.ButAsync(text, multilineTextArg, tableArg, keyword));
    }
}
