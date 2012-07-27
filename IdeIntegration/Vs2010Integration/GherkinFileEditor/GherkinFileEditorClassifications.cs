using Microsoft.VisualStudio.Text.Classification;

namespace TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor
{
    public class GherkinFileEditorClassifications
    {
        public readonly IClassificationType Keyword;
        public readonly IClassificationType Comment;
        public readonly IClassificationType Tag;
        public readonly IClassificationType MultilineText;
        public readonly IClassificationType Placeholder;
        public readonly IClassificationType ScenarioTitle;
        public readonly IClassificationType FeatureTitle;
        public readonly IClassificationType TableCell;
        public readonly IClassificationType TableHeader;
        public readonly IClassificationType Description;
        public readonly IClassificationType StepText;
        public readonly IClassificationType UnboundStepText;
        public readonly IClassificationType StepArgument;

        public GherkinFileEditorClassifications(IClassificationTypeRegistryService registry)
        {
            Keyword = registry.GetClassificationType("gherkin.keyword");
            Comment = registry.GetClassificationType("gherkin.comment");
            Tag = registry.GetClassificationType("gherkin.tag");
            MultilineText = registry.GetClassificationType("gherkin.multilinetext");
            Placeholder = registry.GetClassificationType("gherkin.placeholder");
            ScenarioTitle = registry.GetClassificationType("gherkin.scenariotitle");
            FeatureTitle = registry.GetClassificationType("gherkin.featuretitle");
            TableCell = registry.GetClassificationType("gherkin.tablecell");
            TableHeader = registry.GetClassificationType("gherkin.tableheader");
            Description = registry.GetClassificationType("gherkin.description");
            StepText = registry.GetClassificationType("gherkin.steptext");
            UnboundStepText = registry.GetClassificationType("gherkin.unboundsteptext");
            StepArgument = registry.GetClassificationType("gherkin.stepargument");
        }
    }
}