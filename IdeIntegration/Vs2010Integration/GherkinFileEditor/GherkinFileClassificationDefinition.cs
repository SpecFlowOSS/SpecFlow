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

        // exports a classification type for the Gherkin tags: gherkin.tag
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.tag")]
        internal static ClassificationTypeDefinition GherkinTagClassifierType = null;

        // exports a classification type for the Gherkin Scenaroio Outline placeholders: gherkin.placeholder
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.placeholder")]
        [BaseDefinition("string")]
        internal static ClassificationTypeDefinition GherkinPlaceholderClassifierType = null;

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

    }
}
