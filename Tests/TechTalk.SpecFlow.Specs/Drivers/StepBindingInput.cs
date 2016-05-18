namespace TechTalk.SpecFlow.Specs.Drivers
{
    public class StepBindingInput
    {
        public ScenarioBlock ScenarioBlock { get; private set; }
        public string Regex { get; private set; }
        public string Code { get; private set; }
        public string Parameters { get; set; }

        public StepBindingInput(ScenarioBlock scenarioBlock, string regex, string code)
        {
            ScenarioBlock = scenarioBlock;
            Regex = regex;
            Code = code;
        }
    }
}