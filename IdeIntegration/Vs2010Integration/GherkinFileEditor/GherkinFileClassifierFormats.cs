using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor
{
    // exports a classification format for the classification type gherkin.tag
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.keyword")]
    [Name("gherkin.keyword")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class GherkinKeywordClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinKeywordClassificationFormat()
        {
            this.DisplayName = "Gherkin Keyword"; 
        }
    }

    // exports a classification format for the classification type gherkin.tag
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.comment")]
    [Name("gherkin.comment")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class GherkinCommentClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinCommentClassificationFormat()
        {
            this.DisplayName = "Gherkin Comment";
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
            this.IsItalic = true;
        }
    }

    // exports a classification format for the classification type gherkin.multilinetext
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.multilinetext")]
    [Name("gherkin.multilinetext")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class GherkinMultilineTextClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinMultilineTextClassificationFormat()
        {
            this.DisplayName = "Gherkin Multi-line Text Argument";
        }
    }

    // exports a classification format for the classification type gherkin.tablecell
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.tablecell")]
    [Name("gherkin.tablecell")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class GherkinTableCellClassificationFormat : ClassificationFormatDefinition
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
        }
    }

    // exports a classification format for the classification type gherkin.description
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.description")]
    [Name("gherkin.description")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class GherkinDescriptionClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinDescriptionClassificationFormat()
        {
            this.DisplayName = "Gherkin Feature/Scenario Description";
            this.IsItalic = true;
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

    // exports a classification format for the classification type gherkin.placeholder
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.featuretitle")]
    [Name("gherkin.featuretitle")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class GherkinFeatureTitleClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinFeatureTitleClassificationFormat()
        {
            this.DisplayName = "Gherkin Feature Title"; 
        }
    }

    // exports a classification format for the classification type gherkin.steptext
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.steptext")]
    [Name("gherkin.steptext")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class GherkinStepTextClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinStepTextClassificationFormat()
        {
            this.DisplayName = "Gherkin Step Text"; 
        }
    }
	
	// exports a classification format for the classification type gherkin.unboundsteptext
	[Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.unboundsteptext")]
    [Name("gherkin.unboundsteptext")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class GherkinUnboundStepTextClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinUnboundStepTextClassificationFormat()
        {
            this.DisplayName = "Gherkin Unbound Step Text";
            this.ForegroundColor = Color.FromRgb(105, 54, 153);
        }
    }

	// exports a classification format for the classification type gherkin.stepargument
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.stepargument")]
    [Name("gherkin.stepargument")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class GherkinStepArgumentClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinStepArgumentClassificationFormat()
        {
            this.DisplayName = "Gherkin Step Argument";
            this.ForegroundColor = Color.FromRgb(100, 100, 100);
            this.IsItalic = true;
        }
    }

    // exports a classification format for the classification type gherkin.placeholder
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.placeholder")]
    [Name("gherkin.placeholder")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class GherkinPlaceholderClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinPlaceholderClassificationFormat()
        {
            this.DisplayName = "Gherkin Scenario Outline Placeholder"; 
        }
    }
}
