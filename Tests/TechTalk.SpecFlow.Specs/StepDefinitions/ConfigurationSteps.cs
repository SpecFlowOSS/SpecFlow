using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class ConfigurationSteps
    {
        private readonly ConfigurationDriver _configurationDriver;
        private readonly CompilationResultDriver _compilationResultDriver;

        public ConfigurationSteps(ConfigurationDriver configurationDriver, CompilationResultDriver compilationResultDriver)
        {
            _configurationDriver = configurationDriver;
            _compilationResultDriver = compilationResultDriver;
        }

        [Then(@"the app\.config is used for configuration")]
        public void ThenTheApp_ConfigIsUsedForConfiguration()
        {
            _compilationResultDriver.CheckSolutionShouldUseAppConfig();
        }

        [Then(@"the specflow\.json is used for configuration")]
        public void ThenTheSpecFlow_JsonIsUsedForConfiguration()
        {
            _compilationResultDriver.CheckSolutionShouldUseSpecFlowJson();
        }

        [Given(@"the feature language is '(.*)'")]
        public void GivenTheFeatureLanguageIs(string featureLanguage)
        {
            _configurationDriver.SetFeatureLanguage(featureLanguage);
        }
    }
}
