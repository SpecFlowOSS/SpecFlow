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
        public const bool EnableTracingDefaultValue = false;
        public const string TracingCategoriesDefaultValue = "all";

        private static T GetGeneralOption<T>(DTE dte, string optionName, T defaultValue = default(T))
        {
            return VsxHelper.GetOption(dte, SPECFLOW_OPTIONS_CATEGORY, SPECFLOW_GENERAL_OPTIONS_PAGE, optionName, defaultValue);
        }

        private static IntegrationOptions GetOptions(DTE dte)
        {
            IntegrationOptions options = new IntegrationOptions
                                          {
                                              EnableSyntaxColoring = GetGeneralOption<bool>(dte, "EnableSyntaxColoring", EnableSyntaxColoringDefaultValue),
                                              EnableOutlining = GetGeneralOption<bool>(dte, "EnableOutlining", EnableOutliningDefaultValue),
                                              EnableIntelliSense = GetGeneralOption<bool>(dte, "EnableIntelliSense", EnableIntelliSenseDefaultValue),
                                              EnableAnalysis = GetGeneralOption<bool>(dte, "EnableAnalysis", EnableAnalysisDefaultValue),
                                              EnableTableAutoFormat = GetGeneralOption<bool>(dte, "EnableTableAutoFormat", EnableTableAutoFormatDefaultValue),
                                              EnableTracing = GetGeneralOption<bool>(dte, "EnableTracing", EnableTracingDefaultValue),
                                              TracingCategories = GetGeneralOption<string>(dte, "TracingCategories", TracingCategoriesDefaultValue),
                                          };
            return options;
        }

        [Import]
        internal SVsServiceProvider ServiceProvider = null;

        public IntegrationOptions GetOptions()
        {
            var dte = VsxHelper.GetDte(ServiceProvider);
            return GetOptions(dte);
        }
    }
}
