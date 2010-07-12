using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor
{
    // base format for values (table cell, multiline text, placeholder, etc.)
    internal abstract class GherkinValueClassificationFormat : ClassificationFormatDefinition
    {
        protected GherkinValueClassificationFormat()
        {
            this.ForegroundColor = Color.FromRgb(163, 21, 21); // default color for strings
        }
    }

    // exports a classification format for the classification type gherkin.tag
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.tag")]
    [Name("gherkin.tag")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class GherkinTagClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinTagClassificationFormat()
        {
            this.DisplayName = "Gherkin Tag"; 
            this.ForegroundColor = Colors.Gray;
            this.IsItalic = true;
        }
    }

    // exports a classification format for the classification type gherkin.placeholder
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.placeholder")]
    [Name("gherkin.placeholder")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class GherkinPlaceholderClassificationFormat : GherkinValueClassificationFormat
    {
        public GherkinPlaceholderClassificationFormat()
        {
            this.DisplayName = "Gherkin Scenario Outline Placeholder"; 
        }
    }

    // exports a classification format for the classification type gherkin.tablecell
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.tablecell")]
    [Name("gherkin.tablecell")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class GherkinTableCellClassificationFormat : GherkinValueClassificationFormat
    {
        public GherkinTableCellClassificationFormat()
        {
            this.DisplayName = "Gherkin Table Cell"; 
        }
    }

    // exports a classification format for the classification type gherkin.tableheader
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.tableheader")]
    [Name("gherkin.tableheader")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class GherkinTableHeaderClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinTableHeaderClassificationFormat()
        {
            this.DisplayName = "Gherkin Table Header";
            this.IsItalic = true;
            this.ForegroundColor = Colors.DarkGray;
        }
    }

    // exports a classification format for the classification type gherkin.placeholder
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.scenariotitle")]
    [Name("gherkin.scenariotitle")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class GherkinScenarioTitleClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinScenarioTitleClassificationFormat()
        {
            this.DisplayName = "Gherkin Scenario Title"; 
        }
    }
}
