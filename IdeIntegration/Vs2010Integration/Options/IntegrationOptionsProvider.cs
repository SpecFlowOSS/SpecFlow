using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.Options
{
    internal static class IntegrationOptionsProvider
    {
        public const string SPECFLOW_OPTIONS_CATEGORY = "SpecFlow";
        public const string SPECFLOW_GENERAL_OPTIONS_PAGE = "General";

        public const bool EnableSyntaxColoringDefaultValue = true;
        public const bool EnableOutliningDefaultValue = true;
        public const bool EnableIntelliSenseDefaultValue = true;

        private static T GetGeneralOption<T>(DTE dte, string optionName, T defaultValue = default(T))
        {
            return VsxHelper.GetOption<T>(dte, SPECFLOW_OPTIONS_CATEGORY, SPECFLOW_GENERAL_OPTIONS_PAGE, optionName, defaultValue);
        }

        public static IntegrationOptions GetOptions(DTE dte)
        {
            IntegrationOptions options = new IntegrationOptions
                                          {
                                              EnableSyntaxColoring = GetGeneralOption<bool>(dte, "EnableSyntaxColoring", EnableSyntaxColoringDefaultValue),
                                              EnableOutlining = GetGeneralOption<bool>(dte, "EnableOutlining", EnableOutliningDefaultValue),
                                              EnableIntelliSense = GetGeneralOption<bool>(dte, "EnableIntelliSense", EnableIntelliSenseDefaultValue)
                                          };
            return options;
        }
    }
}
