using System.Collections;
using System.IO;
using SpecFlow.Verify.SpecFlowPlugin;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.UnitTestProvider;
using VerifyTests;

[assembly: RuntimePlugin(typeof(VerifyRuntimePlugin))]

namespace SpecFlow.Verify.SpecFlowPlugin;

public class VerifyRuntimePlugin : IRuntimePlugin
{
    public void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters, UnitTestProviderConfiguration unitTestProviderConfiguration)
    {
        runtimePluginEvents.CustomizeGlobalDependencies += RuntimePluginEvents_CustomizeGlobalDependencies;
    }

    private void RuntimePluginEvents_CustomizeGlobalDependencies(object sender, CustomizeGlobalDependenciesEventArgs e)
    {
        var runtimePluginTestExecutionLifecycleEvents = e.ObjectContainer.Resolve<RuntimePluginTestExecutionLifecycleEvents>();
        runtimePluginTestExecutionLifecycleEvents.BeforeScenario += RuntimePluginTestExecutionLifecycleEvents_BeforeScenario;
    }

    private void RuntimePluginTestExecutionLifecycleEvents_BeforeScenario(object sender, RuntimePluginBeforeScenarioEventArgs e)
    {
        var scenarioContext = e.ObjectContainer.Resolve<ScenarioContext>();
        var featureContext = e.ObjectContainer.Resolve<FeatureContext>();

        VerifierSettings.DerivePathInfo(
            (sourceFile, projectDirectory, type, method) =>
            {
                string scenarioInfoTitle = scenarioContext.ScenarioInfo.Title;

                foreach (DictionaryEntry scenarioInfoArgument in scenarioContext.ScenarioInfo.Arguments)
                {
                    scenarioInfoTitle += "_" + scenarioInfoArgument.Value;
                }

                return new PathInfo(
                    Path.Combine(projectDirectory, featureContext.FeatureInfo.FolderPath),
                    featureContext.FeatureInfo.Title,
                    scenarioInfoTitle);
            });
    }
}
