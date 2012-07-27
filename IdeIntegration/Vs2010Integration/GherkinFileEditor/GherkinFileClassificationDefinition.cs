using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor
{
    internal static class GherkinFileClassificationDefinition
    {
        // export a new content format: gherkin
        [Export]
        [Name("gherkin")]
        [BaseDefinition("text")]
        internal static ContentTypeDefinition diffContentTypeDefinition = null;

        // export a file extension for the 'gherkin' format: .feature
        [Export]
        [FileExtension(".feature")]
        [ContentType("gherkin")]
        internal static FileExtensionToContentTypeDefinition patchFileExtensionDefinition = null;

        // exports a classification type for the Gherkin keywords: gherkin.keyword
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.keyword")]
        [BaseDefinition("keyword")]
        internal static ClassificationTypeDefinition GherkinKeywordClassifierType = null;

        // exports a classification type for the Gherkin comments: gherkin.comment
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.comment")]
        [BaseDefinition("comment")]
        internal static ClassificationTypeDefinition GherkinCommentClassifierType = null;

        // exports a classification type for the Gherkin tags: gherkin.tag
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.tag")]
        [BaseDefinition("symbol definition")]
        internal static ClassificationTypeDefinition GherkinTagClassifierType = null;

        // exports a classification type for the Gherkin multi-line text arguments: gherkin.multilinetext
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.multilinetext")]
        [BaseDefinition("string")]
        internal static ClassificationTypeDefinition GherkinMultilineTextClassifierType = null;

        // exports a classification type for the Gherkin Table Header: gherkin.tableheader
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.tableheader")]
        internal static ClassificationTypeDefinition GherkinTableHeaderClassifierType = null;

        // exports a classification type for the Gherkin Table Cell: gherkin.tablecell
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.tablecell")]
        [BaseDefinition("string")]
        internal static ClassificationTypeDefinition GherkinTableCellClassifierType = null;

        // exports a classification type for the Gherkin Scenario Title: gherkin.scenariotitle
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.scenariotitle")]
        internal static ClassificationTypeDefinition GherkinScenarioTitleClassifierType = null;

        // exports a classification type for the Gherkin Feature Title: gherkin.featuretitle
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.featuretitle")]
        internal static ClassificationTypeDefinition GherkinFeatureTitleClassifierType = null;

        // exports a classification type for the Gherkin Feature/Scenario Description: gherkin.description
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.description")]
        internal static ClassificationTypeDefinition GherkinDescriptionClassifierType = null;

        // exports a classification type for the Gherkin Feature Title: gherkin.steptext
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.steptext")]
        internal static ClassificationTypeDefinition GherkinStepTextClassifierType = null;
		
        // exports a classification type for the Gherkin Feature Title: gherkin.unboundsteptext
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.unboundsteptext")]
        internal static ClassificationTypeDefinition GherkinUnboundStepTextClassifierType = null;

        // exports a classification type for the Gherkin Feature Title: gherkin.stepargument
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.stepargument")]
        internal static ClassificationTypeDefinition GherkinStepArgumentClassifierType = null;

        // exports a classification type for the Gherkin Scenaroio Outline placeholders: gherkin.placeholder
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.placeholder")]
        [BaseDefinition("string")]
        internal static ClassificationTypeDefinition GherkinPlaceholderClassifierType = null;
    }
}
