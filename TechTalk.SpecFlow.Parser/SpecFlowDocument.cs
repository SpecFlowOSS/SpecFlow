using Gherkin.Ast;

namespace TechTalk.SpecFlow.Parser
{
    public class SpecFlowDocument : GherkinDocument
    {
        public SpecFlowDocument(SpecFlowFeature feature, Comment[] comments, SpecFlowDocumentLocation documentLocation) : base(feature, comments)
        {
            DocumentLocation = documentLocation;
        }

        public SpecFlowFeature SpecFlowFeature => (SpecFlowFeature) Feature;

        public SpecFlowDocumentLocation DocumentLocation { get; private set; }

        public string SourceFilePath => DocumentLocation?.SourceFilePath;
    }
}
