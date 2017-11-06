using System;
using System.Configuration;
using BoDi;

namespace TechTalk.SpecFlow.Configuration.AppConfig
{
    public partial class GeneratorConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("dependencies", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        [ConfigurationCollection(typeof(ContainerRegistrationCollection), AddItemName = "register")]
        public ContainerRegistrationCollection Dependencies
        {
            get { return (ContainerRegistrationCollection)this["dependencies"]; }
            set { this["dependencies"] = value; }
        }

        [ConfigurationProperty("allowDebugGeneratedFiles", DefaultValue = ConfigDefaults.AllowDebugGeneratedFiles, IsRequired = false)]
        public bool AllowDebugGeneratedFiles
        {
            get { return (bool)this["allowDebugGeneratedFiles"]; }
            set { this["allowDebugGeneratedFiles"] = value; }
        }

        [ConfigurationProperty("allowRowTests", DefaultValue = ConfigDefaults.AllowRowTests, IsRequired = false)]
        public bool AllowRowTests
        {
            get { return (bool)this["allowRowTests"]; }
            set { this["allowRowTests"] = value; }
        }

        [Obsolete("Async tests are not part of the core library from v2")]
        [ConfigurationProperty("generateAsyncTests", DefaultValue = false, IsRequired = false)]
        public bool GenerateAsyncTests
        {
            get { return (bool)this["generateAsyncTests"]; }
            set { this["generateAsyncTests"] = value; }
        }

        [ConfigurationProperty("path", DefaultValue = ConfigDefaults.GeneratorPath, IsRequired = false)]
        public string GeneratorPath
        {
            get { return (string)this["path"]; }
            set { this["path"] = value; }
        }

        [ConfigurationProperty("markFeaturesParallelizable", DefaultValue = ConfigDefaults.MarkFeaturesParallelizable, IsRequired = false)]
        public bool MarkFeaturesParallelizable
        {
            get { return (bool)this["markFeaturesParallelizable"]; }
            set { this["markFeaturesParallelizable"] = value; }
        }

        [ConfigurationProperty("skipParallelizableMarkerForTags", IsRequired = false, Options = ConfigurationPropertyOptions.None)]
        [ConfigurationCollection(typeof(TagCollection), AddItemName = "tag")]
        public TagCollection SkipParallelizableMarkerForTags
        {
            get { return (TagCollection)this["skipParallelizableMarkerForTags"]; }
            set { this["skipParallelizableMarkerForTags"] = value; }
        }
    }
}