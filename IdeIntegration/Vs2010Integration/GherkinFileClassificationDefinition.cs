using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace GherkinFileClassifier
{
    internal static class GherkinFileClassificationDefinition
    {
        [Export]
        [Name("gherkin")]
        [BaseDefinition("text")]
        internal static ContentTypeDefinition diffContentTypeDefinition = null;

        [Export]
        [FileExtension(".feature")]
        [ContentType("gherkin")]
        internal static FileExtensionToContentTypeDefinition patchFileExtensionDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.tag")]
        internal static ClassificationTypeDefinition GherkinTagClassifierType = null;
    }
}
