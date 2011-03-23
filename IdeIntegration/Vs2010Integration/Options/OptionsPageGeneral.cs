using System;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace TechTalk.SpecFlow.Vs2010Integration.Options
{
    /// <summary>
    // Extends a standard dialog functionality for implementing ToolsOptions pages, 
    // with support for the Visual Studio automation model, Windows Forms, and state 
    // persistence through the Visual Studio settings mechanism.
    /// </summary>
    [Guid("D41B81C9-8501-4124-B75F-0F194E37178C")]
    [ComVisible(true)]
    public class OptionsPageGeneral : DialogPage
    {
        [Category("Analysis Settings")]
        [Description("Controls whether SpecFlow should collect binding information and step suggestions from the feature files.")]
        [DisplayName(@"Enable project-wide analysis")]
        [DefaultValue(IntegrationOptionsProvider.EnableAnalysisDefaultValue)]
        public bool EnableAnalysis { get; set; }

        private bool enableSyntaxColoring = true;
        [Category("Editor Settings")]
        [Description("Controls whether the different syntax elements of the feature files should be indicated in the editor.")]
        [DisplayName(@"Enable Syntax Coloring")]
        [DefaultValue(IntegrationOptionsProvider.EnableSyntaxColoringDefaultValue)]
        [RefreshProperties(RefreshProperties.All)]
        public bool EnableSyntaxColoring
        {
            get { return enableSyntaxColoring; }
            set
            {
                enableSyntaxColoring = value;
                if (!value)
                {
                    EnableOutlining = false;
                    EnableIntelliSense = false;
                }
            }
        }

        [Category("Editor Settings")]
        [Description("Controls whether the scenario blocks of the feature files should be outlined in the editor.")]
        [DisplayName(@"Enable Outlining")]
        [DefaultValue(IntegrationOptionsProvider.EnableOutliningDefaultValue)]
        public bool EnableOutlining { get; set; }

        [Category("Editor Settings")]
        [Description("Controls whether completion lists should be displayed for the feature files.")]
        [DisplayName(@"Enable IntelliSense")]
        [DefaultValue(IntegrationOptionsProvider.EnableIntelliSenseDefaultValue)]
        public bool EnableIntelliSense { get; set; }

        public OptionsPageGeneral()
        {
            EnableAnalysis = IntegrationOptionsProvider.EnableAnalysisDefaultValue;
            EnableSyntaxColoring = IntegrationOptionsProvider.EnableSyntaxColoringDefaultValue;
            EnableOutlining = IntegrationOptionsProvider.EnableOutliningDefaultValue;
            EnableIntelliSense = IntegrationOptionsProvider.EnableIntelliSenseDefaultValue;
        }
    }
}
