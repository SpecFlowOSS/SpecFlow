using System;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using TechTalk.SpecFlow.IdeIntegration.Options;

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
        [Description("Controls whether SpecFlow should collect binding information and step suggestions from the feature files. (restart required)")]
        [DisplayName(@"Enable project-wide analysis")]
        [DefaultValue(IntegrationOptionsProvider.EnableAnalysisDefaultValue)]
        public bool EnableAnalysis { get; set; }

        [Category("Analysis Settings")]
        [Description("Controls whether SpecFlow Visual Studio integration should offer re-generating the feature files on configuration change.")]
        [DisplayName(@"Disable re-generate feature file popup")]
        [DefaultValue(IntegrationOptionsProvider.DisableRegenerateFeatureFilePopupOnConfigChangeDefaultValue)]
        public bool DisableRegenerateFeatureFilePopupOnConfigChange { get; set; }

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

        [Category("Editor Settings")]
        [Description("Controls whether the step definition match status should be indicated with a different color in the editor. (beta)")]
        [DisplayName(@"Enable Step Match Coloring")]
        [DefaultValue(IntegrationOptionsProvider.EnableStepMatchColoringDefaultValue)]
        public bool EnableStepMatchColoring { get; set; }

        [Category("Editor Settings")]
        [Description("Controls whether the tables should be formatted automatically when you type \"|\" character.")]
        [DisplayName(@"Enable Table Formatting")]
        [DefaultValue(IntegrationOptionsProvider.EnableTableAutoFormatDefaultValue)]
        public bool EnableTableAutoFormat { get; set; }

        [Category("Tracing")]
        [Description("Controls whether diagnostic trace messages should be emitted to the output window.")]
        [DisplayName(@"Enable Tracing")]
        [DefaultValue(IntegrationOptionsProvider.EnableTracingDefaultValue)]
        public bool EnableTracing { get; set; }

        [Category("Tracing")]
        [Description("Specifies the enabled the tracing categories in a comma-seperated list. Use \"all\" to trace all categories.")]
        [DisplayName(@"Tracing Categories")]
        [DefaultValue(IntegrationOptionsProvider.TracingCategoriesDefaultValue)]
        public string TracingCategories { get; set; }

        [Category("Test Execution")]
        [Description("Specifies the test runner tool to be used to execute the SpecFlow scenarios.")]
        [DisplayName(@"Test Runner Tool")]
        [DefaultValue(IntegrationOptionsProvider.TestRunnerToolDefaultValue)]
        public TestRunnerTool TestRunnerTool { get; set; }

        public OptionsPageGeneral()
        {
            EnableAnalysis = IntegrationOptionsProvider.EnableAnalysisDefaultValue;
            EnableSyntaxColoring = IntegrationOptionsProvider.EnableSyntaxColoringDefaultValue;
            EnableOutlining = IntegrationOptionsProvider.EnableOutliningDefaultValue;
            EnableIntelliSense = IntegrationOptionsProvider.EnableIntelliSenseDefaultValue;
            EnableTableAutoFormat = IntegrationOptionsProvider.EnableTableAutoFormatDefaultValue;
            EnableStepMatchColoring = IntegrationOptionsProvider.EnableStepMatchColoringDefaultValue;
            EnableTracing = IntegrationOptionsProvider.EnableTracingDefaultValue;
            TracingCategories = IntegrationOptionsProvider.TracingCategoriesDefaultValue;
            TestRunnerTool = IntegrationOptionsProvider.TestRunnerToolDefaultValue;
            DisableRegenerateFeatureFilePopupOnConfigChange = IntegrationOptionsProvider.DisableRegenerateFeatureFilePopupOnConfigChangeDefaultValue;
        }
    }
}
