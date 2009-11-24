using System;
using System.Globalization;
using System.Linq;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Parser.UnitTestProvider;

namespace TechTalk.SpecFlow.Parser.Configuration
{
    public class GeneratorConfiguration
    {
        //language settings
        public CultureInfo FeatureLanguage { get; set; } //TODO: use
        public CultureInfo ToolLanguage { get; set; } //TODO: use

        //unit test framework settings
        public Type GeneratorUnitTestProviderType { get; set; }

        // generator settings
        public bool AllowDebugGeneratedFiles { get; set; }
        
        public GeneratorConfiguration()
        {
            FeatureLanguage = CultureInfo.GetCultureInfo(ConfigDefaults.FeatureLanguage);
            ToolLanguage = string.IsNullOrEmpty(ConfigDefaults.ToolLanguage) ?
                FeatureLanguage : 
                CultureInfo.GetCultureInfo(ConfigDefaults.ToolLanguage);

            SetUnitTestDefaultsByName(ConfigDefaults.UnitTestProviderName);

            AllowDebugGeneratedFiles = ConfigDefaults.AllowDebugGeneratedFiles;
        }

        internal void UpdateFromConfigFile(ConfigurationSectionHandler configSection)
        {
            if (configSection == null) throw new ArgumentNullException("configSection");

            if (configSection.Language != null)
            {
                FeatureLanguage = CultureInfo.GetCultureInfo(configSection.Language.Feature);
                ToolLanguage = string.IsNullOrEmpty(configSection.Language.Tool) ?
                    FeatureLanguage :
                    CultureInfo.GetCultureInfo(configSection.Language.Tool);
            }

            if (configSection.UnitTestProvider != null)
            {
                SetUnitTestDefaultsByName(configSection.UnitTestProvider.Name);

                if (!string.IsNullOrEmpty(configSection.UnitTestProvider.GeneratorProvider))
                    GeneratorUnitTestProviderType = GetTypeConfig(configSection.UnitTestProvider.GeneratorProvider);

                //TODO: config.CheckUnitTestConfig();
            }
        }

        private static Type GetTypeConfig(string typeName)
        {
            //TODO: nicer error message?
            return Type.GetType(typeName, true);
        }

        private void SetUnitTestDefaultsByName(string name)
        {
            switch (name.ToLowerInvariant())
            {
                case "nunit":
                    GeneratorUnitTestProviderType = typeof(NUnitTestConverter);
                    break;
                case "mstest":
                    GeneratorUnitTestProviderType = typeof(MsTestGeneratorProvider);
                    break;
                default:
                    GeneratorUnitTestProviderType = null;
                    break;
            }

        }
    }
}
