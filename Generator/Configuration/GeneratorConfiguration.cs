using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using BoDi;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.UnitTestProvider;

namespace TechTalk.SpecFlow.Generator.Configuration
{
    public class GeneratorConfiguration
    {
        public ContainerRegistrationCollection CustomDependencies { get; set; }

        //language settings
        public CultureInfo FeatureLanguage { get; set; }
        public CultureInfo ToolLanguage { get; set; }

        //unit test framework settings
        public string GeneratorUnitTestProvider { get; set; }

        // generator settings
        public bool AllowDebugGeneratedFiles { get; set; }
        public bool AllowRowTests { get; set; }
        public bool GenerateAsyncTests { get; set; }
        public string GeneratorPath { get; set; }

        public bool UsesPlugins { get; private set; }

        public GeneratorConfiguration()
        {
            FeatureLanguage = CultureInfo.GetCultureInfo(ConfigDefaults.FeatureLanguage);
            ToolLanguage = CultureInfo.GetCultureInfo(ConfigDefaults.FeatureLanguage);

            GeneratorUnitTestProvider = ConfigDefaults.UnitTestProviderName;

            AllowDebugGeneratedFiles = ConfigDefaults.AllowDebugGeneratedFiles;
            AllowRowTests = ConfigDefaults.AllowRowTests;
            GenerateAsyncTests = ConfigDefaults.GenerateAsyncTests;
            GeneratorPath = ConfigDefaults.GeneratorPath;

            UsesPlugins = false;
        }

        internal void UpdateFromConfigFile(ConfigurationSectionHandler configSection)
        {
            if (configSection == null) throw new ArgumentNullException("configSection");

            if (IsSpecified(configSection.Language))
            {
                FeatureLanguage = CultureInfo.GetCultureInfo(configSection.Language.Feature);
                ToolLanguage = string.IsNullOrEmpty(configSection.Language.Tool) ?
                    CultureInfo.GetCultureInfo(configSection.Language.Feature) :
                    CultureInfo.GetCultureInfo(configSection.Language.Tool);
            }

            if (IsSpecified(configSection.Generator))
            {
                AllowDebugGeneratedFiles = configSection.Generator.AllowDebugGeneratedFiles;
                AllowRowTests = configSection.Generator.AllowRowTests;
                GenerateAsyncTests = configSection.Generator.GenerateAsyncTests;
                GeneratorPath = configSection.Generator.GeneratorPath;

                if (IsSpecified(configSection.Generator.Dependencies))
                {
                    this.CustomDependencies = configSection.Generator.Dependencies;
                    UsesPlugins = true;
                }
            }

            if (IsSpecified(configSection.UnitTestProvider))
            {
                if (!string.IsNullOrEmpty(configSection.UnitTestProvider.GeneratorProvider))
                {
                    //compatibility mode, we simulate a custom dependency

                    if (this.CustomDependencies == null)
                        this.CustomDependencies = new ContainerRegistrationCollection();

                    this.GeneratorUnitTestProvider = "custom";
                    this.CustomDependencies.Add(configSection.UnitTestProvider.GeneratorProvider, typeof(IUnitTestGeneratorProvider).AssemblyQualifiedName, this.GeneratorUnitTestProvider);

                    UsesPlugins = true;
                }
                else
                {
                    this.GeneratorUnitTestProvider = configSection.UnitTestProvider.Name;
                }
            }

            if (IsSpecified(configSection.Plugins))
                UsesPlugins = true;
        }

        private bool IsSpecified(ConfigurationElement configurationElement)
        {
            return configurationElement != null && configurationElement.ElementInformation.IsPresent;
        }

        #region Equality

        protected bool Equals(GeneratorConfiguration other)
        {
            return Equals(FeatureLanguage, other.FeatureLanguage) && Equals(ToolLanguage, other.ToolLanguage) && string.Equals(GeneratorUnitTestProvider, other.GeneratorUnitTestProvider) && AllowDebugGeneratedFiles.Equals(other.AllowDebugGeneratedFiles) && AllowRowTests.Equals(other.AllowRowTests) && GenerateAsyncTests.Equals(other.GenerateAsyncTests) && string.Equals(GeneratorPath, other.GeneratorPath) && UsesPlugins.Equals(other.UsesPlugins);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GeneratorConfiguration) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (FeatureLanguage != null ? FeatureLanguage.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ToolLanguage != null ? ToolLanguage.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (GeneratorUnitTestProvider != null ? GeneratorUnitTestProvider.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ AllowDebugGeneratedFiles.GetHashCode();
                hashCode = (hashCode*397) ^ AllowRowTests.GetHashCode();
                hashCode = (hashCode*397) ^ GenerateAsyncTests.GetHashCode();
                hashCode = (hashCode*397) ^ (GeneratorPath != null ? GeneratorPath.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ UsesPlugins.GetHashCode();
                return hashCode;
            }
        }

        #endregion
    }
}