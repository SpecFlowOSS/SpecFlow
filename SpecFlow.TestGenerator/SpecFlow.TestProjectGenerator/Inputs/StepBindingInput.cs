namespace SpecFlow.TestProjectGenerator.Inputs
{
    public class StepBindingInput
    {
        public StepBindingInput(string scenarioBlock, string regex, string cSharpCode, string vbNetCode)
        {
            ScenarioBlock = scenarioBlock;
            Regex = regex;
            CSharpCode = cSharpCode;
            VBNetCode = vbNetCode;
        }

        public string ScenarioBlock { get; }
        public string Regex { get; }
        public string CSharpCode { get; }
        public string VBNetCode { get; }
    }
}