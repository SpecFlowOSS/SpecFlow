using TechTalk.SpecFlow.Parser.Gherkin;

namespace TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor
{
    internal class PartialListeningDoneException : ScanningCancelledException
    {
        public ScenarioEditorInfo FirstUnchangedScenario { get; private set; }

        public PartialListeningDoneException(ScenarioEditorInfo firstUnchangedScenario)
        {
            FirstUnchangedScenario = firstUnchangedScenario;
        }
    }
}