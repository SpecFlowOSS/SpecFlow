﻿using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using MiniDi;
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
        public Type GeneratorUnitTestProviderType { get; set; }

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

            SetUnitTestDefaultsByName(ConfigDefaults.UnitTestProviderName);

            AllowDebugGeneratedFiles = ConfigDefaults.AllowDebugGeneratedFiles;
            AllowRowTests = ConfigDefaults.AllowRowTests;
            GenerateAsyncTests = ConfigDefaults.GenerateAsyncTests;
            GeneratorPath = ConfigDefaults.GeneratorPath;

            UsesPlugins = false;
        }

        internal void UpdateFromConfigFile(ConfigurationSectionHandler configSection, bool loadPlugins)
        {
            if (configSection == null) throw new ArgumentNullException("configSection");

            if (configSection.Language != null)
            {
                FeatureLanguage = CultureInfo.GetCultureInfo(configSection.Language.Feature);
                ToolLanguage = string.IsNullOrEmpty(configSection.Language.Tool) ? FeatureLanguage :
                    CultureInfo.GetCultureInfo(configSection.Language.Tool);
            }

            if (configSection.UnitTestProvider != null)
            {
                SetUnitTestDefaultsByName(configSection.UnitTestProvider.Name);

                if (!string.IsNullOrEmpty(configSection.UnitTestProvider.GeneratorProvider))
                {
                    if (loadPlugins)
                        GeneratorUnitTestProviderType = GetTypeConfig(configSection.UnitTestProvider.GeneratorProvider);
                    UsesPlugins = true;
                }

                //TODO: config.CheckUnitTestConfig();
            }

            if (configSection.Generator != null)
            {
                AllowDebugGeneratedFiles = configSection.Generator.AllowDebugGeneratedFiles;
                AllowRowTests = configSection.Generator.AllowRowTests;
                GenerateAsyncTests = configSection.Generator.GenerateAsyncTests;
                GeneratorPath = configSection.Generator.GeneratorPath;
            }

            if (configSection.Generator != null && configSection.Generator.Dependencies != null)
            {
                CustomDependencies = configSection.Generator.Dependencies;
                UsesPlugins = true; //TODO: this calculation can be refined later
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
                    GeneratorUnitTestProviderType = typeof(NUnitTestGeneratorProvider);
                    break;
                case "mbunit":
                    GeneratorUnitTestProviderType = typeof(MbUnitTestGeneratorProvider);
                    break;
                case "mbunitv3":
                    GeneratorUnitTestProviderType = typeof(MbUnitTestv3GeneratorProvider);
                    break;
                case "xunit":
                    GeneratorUnitTestProviderType = typeof(XUnitTestGeneratorProvider);
                    break;
                case "mstest":
                    GeneratorUnitTestProviderType = typeof(MsTestGeneratorProvider);
                    break;
                case "mstest.2010":
                    GeneratorUnitTestProviderType = typeof(MsTest2010GeneratorProvider);
                    break;
                case "mstest.silverlight":
                case "mstest.silverlight3":
                case "mstest.silverlight4":
                case "mstest.windowsphone7":
                    GeneratorUnitTestProviderType = typeof(MsTestSilverlightGeneratorProvider);
                    break;
                default:
                    GeneratorUnitTestProviderType = null;
                    break;
            }

        }

        #region Equality

        public bool Equals(GeneratorConfiguration other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.FeatureLanguage, FeatureLanguage) && Equals(other.ToolLanguage, ToolLanguage) && Equals(other.GeneratorUnitTestProviderType, GeneratorUnitTestProviderType) && other.AllowDebugGeneratedFiles.Equals(AllowDebugGeneratedFiles) && other.AllowRowTests.Equals(AllowRowTests);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (GeneratorConfiguration)) return false;
            return Equals((GeneratorConfiguration) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (FeatureLanguage != null ? FeatureLanguage.GetHashCode() : 0);
                result = (result*397) ^ (ToolLanguage != null ? ToolLanguage.GetHashCode() : 0);
                result = (result*397) ^ (GeneratorUnitTestProviderType != null ? GeneratorUnitTestProviderType.GetHashCode() : 0);
                result = (result*397) ^ AllowDebugGeneratedFiles.GetHashCode();
                result = (result*397) ^ AllowRowTests.GetHashCode();
                return result;
            }
        }

        #endregion
    }
}