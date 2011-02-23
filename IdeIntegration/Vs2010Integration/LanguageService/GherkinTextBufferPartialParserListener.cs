using System.Linq;
using Microsoft.VisualStudio.Text;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.Gherkin;
using TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    internal class PartialListeningDoneException : ScanningCancelledException
    {
        public IScenarioBlock FirstUnchangedScenario { get; private set; }

        public PartialListeningDoneException(IScenarioBlock firstUnchangedScenario)
        {
            FirstUnchangedScenario = firstUnchangedScenario;
        }
    }

    internal class GherkinTextBufferPartialParserListener : GherkinTextBufferParserListenerBase
    {
        private readonly IGherkinFileScope previousScope;
        private readonly int changeLastLine;
        private readonly int changeLineDelta;

        public GherkinTextBufferPartialParserListener(GherkinDialect gherkinDialect, ITextSnapshot textSnapshot, GherkinFileEditorClassifications classifications, IGherkinFileScope previousScope, int changeLastLine, int changeLineDelta)
            : base(gherkinDialect, textSnapshot, classifications)
        {
            this.previousScope = previousScope;
            this.changeLastLine = changeLastLine;
            this.changeLineDelta = changeLineDelta;
        }

        protected override void OnScenarioBlockCreating(int editorLine)
        {
            base.OnScenarioBlockCreating(editorLine);

            if (editorLine > changeLastLine)
            {
                var firstUnchangedScenario = previousScope.ScenarioBlocks.FirstOrDefault(
                    prevScenario => GherkinFileScopeExtensions.GetStartLine(prevScenario) + changeLineDelta == editorLine);

                if (firstUnchangedScenario != null)
                    throw new PartialListeningDoneException(firstUnchangedScenario);
            }
        }
    }
}