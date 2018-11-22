using Gherkin.Ast;

namespace TechTalk.SpecFlow.Parser
{
    public class SpecFlowDocument : GherkinDocument
    {
        public SpecFlowDocument(SpecFlowFeature feature, Comment[] comments, string sourceFilePath) : base(feature, comments)
        {
            this.SourceFilePath = sourceFilePath;

        }

        public SpecFlowFeature SpecFlowFeature => (SpecFlowFeature) Feature;

        public string SourceFilePath { get; private set; }

    }
}
