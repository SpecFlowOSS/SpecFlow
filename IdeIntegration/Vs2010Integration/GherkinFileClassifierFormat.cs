using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace GherkinFileClassifier
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.tag")]
    [Name("gherkin.tag")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class GherkinFileClassifierFormat : ClassificationFormatDefinition
    {
        public GherkinFileClassifierFormat()
        {
            this.DisplayName = "Gherkin Tag"; //human readable version of the name
            this.ForegroundColor = Colors.Gray;
            this.IsItalic = true;
        }
    }
}
