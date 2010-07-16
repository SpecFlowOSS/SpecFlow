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
        public readonly IClassificationType TableCell;
        public readonly IClassificationType TableHeader;

        public GherkinFileEditorClassifications(IClassificationTypeRegistryService registry)
        {
            Keyword = registry.GetClassificationType("keyword");
            Comment = registry.GetClassificationType("comment");
            Tag = registry.GetClassificationType("gherkin.tag");
            MultilineText = registry.GetClassificationType("string");
            Placeholder = registry.GetClassificationType("gherkin.placeholder");
            ScenarioTitle = registry.GetClassificationType("gherkin.scenariotitle");
            TableCell = registry.GetClassificationType("gherkin.tablecell");
            TableHeader = registry.GetClassificationType("gherkin.tableheader");
        }
    }
}