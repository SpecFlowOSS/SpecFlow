using System;
using System.ComponentModel.Composition;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.Options
{
    [Export(typeof(IIntegrationOptionsProvider))]
    internal class IntegrationOptionsProvider : IIntegrationOptionsProvider
    {
        public const string SPECFLOW_OPTIONS_CATEGORY = "SpecFlow";
        public const string SPECFLOW_GENERAL_OPTIONS_PAGE = "General";

        public const bool EnableSyntaxColoringDefaultValue = true;
        public const bool EnableOutliningDefaultValue = true;
        public const bool EnableIntelliSenseDefaultValue = true;
        public const bool EnableAnalysisDefaultValue = true;
        public const bool EnableTableAutoFormatDefaultValue = true;
        public const bool EnableStepMatchColoringDefaultValue = true;
        public const bool EnableTracingDefaultValue = false;
        public const string TracingCategoriesDefaultValue = "all";
        public const TestRunnerTool TestRunnerToolDefaultValue = TestRunnerTool.Auto;
        public const bool DisableRegenerateFeatureFilePopupOnConfigChangeDefaultValue = false;

        private DTE dte;

        public IntegrationOptionsProvider()
        {
        }

        public IntegrationOptionsProvider(DTE dte)
        {
            this.dte = dte;
        }

        private static T GetGeneralOption<T>(DTE dte, string optionName, T defaultValue = default(T))
        {
            return VsxHelper.GetOption(dte, SPECFLOW_OPTIONS_CATEGORY, SPECFLOW_GENERAL_OPTIONS_PAGE, optionName, defaultValue);
        }

        private static IntegrationOptions GetOptions(DTE dte)
        {
            IntegrationOptions options = new IntegrationOptions
                                          {
                                              EnableSyntaxColoring = GetGeneralOption(dte, "EnableSyntaxColoring", EnableSyntaxColoringDefaultValue),
                                              EnableOutlining = GetGeneralOption(dte, "EnableOutlining", EnableOutliningDefaultValue),
                                              EnableIntelliSense = GetGeneralOption(dte, "EnableIntelliSense", EnableIntelliSenseDefaultValue),
                                              EnableAnalysis = GetGeneralOption(dte, "EnableAnalysis", EnableAnalysisDefaultValue),
                                              EnableTableAutoFormat = GetGeneralOption(dte, "EnableTableAutoFormat", EnableTableAutoFormatDefaultValue),
                                              EnableStepMatchColoring = GetGeneralOption(dte, "EnableStepMatchColoring", EnableStepMatchColoringDefaultValue),
                                              EnableTracing = GetGeneralOption(dte, "EnableTracing", EnableTracingDefaultValue),
                                              TracingCategories = GetGeneralOption(dte, "TracingCategories", TracingCategoriesDefaultValue),
                                              TestRunnerTool = GetGeneralOption(dte, "TestRunnerTool", TestRunnerToolDefaultValue),
                                              DisableRegenerateFeatureFilePopupOnConfigChange = GetGeneralOption(dte, "DisableRegenerateFeatureFilePopupOnConfigChange", DisableRegenerateFeatureFilePopupOnConfigChangeDefaultValue)
                                          };
            return options;
        }

        [Import]
        internal SVsServiceProvider ServiceProvider
        {
            set { dte = VsxHelper.GetDte(value); }
        }

        public IntegrationOptions GetOptions()
        {
            return GetOptions(dte);
        }
    }
}
