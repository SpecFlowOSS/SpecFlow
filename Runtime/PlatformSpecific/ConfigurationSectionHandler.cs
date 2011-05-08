using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using MiniDi;

namespace TechTalk.SpecFlow.Configuration
{
    partial class ConfigurationSectionHandler : ConfigurationSection
    {
        [ConfigurationProperty("language", IsRequired = false)]
        public LanguageConfigElement Language
        {
            get { return (LanguageConfigElement)this["language"]; }
            set { this["language"] = value; }
        }  
        
        [ConfigurationProperty("bindingCulture", IsRequired = false)]
        public BindingCultureConfigElement BindingCulture
        {
            get { return (BindingCultureConfigElement)this["bindingCulture"]; }
            set { this["bindingCulture"] = value; }
        }

        [ConfigurationProperty("unitTestProvider", IsRequired = false)]
        public UnitTestProviderConfigElement UnitTestProvider
        {
            get { return (UnitTestProviderConfigElement)this["unitTestProvider"]; }
            set { this["unitTestProvider"] = value; }
        }

        [ConfigurationProperty("generator", IsRequired = false)]
        public GeneratorConfigElement Generator
        {
            get { return (GeneratorConfigElement)this["generator"]; }
            set { this["generator"] = value; }
        }

        [ConfigurationProperty("runtime", IsRequired = false)]
        public RuntimeConfigElement Runtime
        {
            get { return (RuntimeConfigElement)this["runtime"]; }
            set { this["runtime"] = value; }
        }

        [ConfigurationProperty("trace", IsRequired = false)]
        public TraceConfigElement Trace
        {
            get { return (TraceConfigElement)this["trace"]; }
            set { this["trace"] = value; }
        }

        [ConfigurationProperty("stepAssemblies", IsDefaultCollection = false, IsRequired = false)]
        [ConfigurationCollection(typeof(StepAssemblyCollection), AddItemName = "stepAssembly")]
        public StepAssemblyCollection StepAssemblies
        {
            get { return (StepAssemblyCollection)this["stepAssemblies"]; }
            set { this["stepAssemblies"] = value; }
        }

        static internal ConfigurationSectionHandler CreateFromXml(string xmlContent)
        {
            ConfigurationSectionHandler section = new ConfigurationSectionHandler();
            section.Init();
            section.Reset(null);
            using (var reader = new XmlTextReader(new StringReader(xmlContent.Trim())))
            {
                section.DeserializeSection(reader);
            }
            section.ResetModified();
            return section;
        }

        static internal ConfigurationSectionHandler CreateFromXml(XmlNode xmlContent)
        {
            ConfigurationSectionHandler section = new ConfigurationSectionHandler();
            section.Init();
            section.Reset(null);
            using (var reader = new XmlNodeReader(xmlContent))
            {
                section.DeserializeSection(reader);
            }
            section.ResetModified();
            return section;
        }
    }

    public class LanguageConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("feature", DefaultValue = "en", IsRequired = false)]
        [RegexStringValidator(@"\w{2}(-\w{2})?")]
        public string Feature
        {
            get { return (String)this["feature"]; }
            set { this["feature"] = value; }
        }

        [ConfigurationProperty("tool", DefaultValue = "", IsRequired = false)]
        [RegexStringValidator(@"\w{2}(-\w{2})?|")]
        public string Tool
        {
            get { return (String)this["tool"]; }
            set { this["tool"] = value; }
        }
    }   
    
    public class BindingCultureConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("name", DefaultValue = "en-US", IsRequired = false)]
        [RegexStringValidator(@"\w{2}(-\w{2})?")]
        public string Name
        {
            get { return (String)this["name"]; }
            set { this["name"] = value; }
        }
    }

    public class UnitTestProviderConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, DefaultValue = "NUnit")]
        [StringValidator(MinLength = 1)]
        public string Name
        {
            get { return (String)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("generatorProvider", DefaultValue = null, IsRequired = false)]
        public string GeneratorProvider
        {
            get { return (string)this["generatorProvider"]; }
            set { this["generatorProvider"] = value; }
        }

        [ConfigurationProperty("runtimeProvider", DefaultValue = null, IsRequired = false)]
        public string RuntimeProvider
        {
            get { return (string)this["runtimeProvider"]; }
            set { this["runtimeProvider"] = value; }
        }
    }

    public class RuntimeConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("detectAmbiguousMatches", DefaultValue = ConfigDefaults.DetectAmbiguousMatches, IsRequired = false)]
        public bool DetectAmbiguousMatches
        {
            get { return (bool)this["detectAmbiguousMatches"]; }
            set { this["detectAmbiguousMatches"] = value; }
        }

        [ConfigurationProperty("stopAtFirstError", DefaultValue = ConfigDefaults.StopAtFirstError, IsRequired = false)]
        public bool StopAtFirstError
        {
            get { return (bool)this["stopAtFirstError"]; }
            set { this["stopAtFirstError"] = value; }
        }

        [ConfigurationProperty("missingOrPendingStepsOutcome", DefaultValue = ConfigDefaults.MissingOrPendingStepsOutcome, IsRequired = false)]
        public MissingOrPendingStepsOutcome MissingOrPendingStepsOutcome
        {
            get { return (MissingOrPendingStepsOutcome)this["missingOrPendingStepsOutcome"]; }
            set { this["missingOrPendingStepsOutcome"] = value; }
        }
    }

    public class GeneratorConfigElement : ConfigurationElement
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

        [ConfigurationProperty("path", DefaultValue = ConfigDefaults.GeneratorPath, IsRequired = false)]
        public string GeneratorPath
        {
            get { return (string)this["path"]; }
            set { this["path"] = value; }
        }
    }

    public class TraceConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("traceSuccessfulSteps", DefaultValue = ConfigDefaults.TraceSuccessfulSteps, IsRequired = false)]
        public bool TraceSuccessfulSteps
        {
            get { return (bool)this["traceSuccessfulSteps"]; }
            set { this["traceSuccessfulSteps"] = value; }
        }

        [ConfigurationProperty("traceTimings", DefaultValue = ConfigDefaults.TraceTimings, IsRequired = false)]
        public bool TraceTimings
        {
            get { return (bool)this["traceTimings"]; }
            set { this["traceTimings"] = value; }
        }

        [ConfigurationProperty("minTracedDuration", DefaultValue = ConfigDefaults.MinTracedDuration, IsRequired = false)]
        public TimeSpan MinTracedDuration
        {
            get { return (TimeSpan)this["minTracedDuration"]; }
            set { this["minTracedDuration"] = value; }
        }

        [ConfigurationProperty("listener", DefaultValue = null, IsRequired = false)]
        public string Listener
        {
            get { return (string)this["listener"]; }
            set { this["listener"] = value; }
        }
    }

    public class StepAssemblyCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new StepAssemblyConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((StepAssemblyConfigElement)element).Assembly;
        }
    }

    public class StepAssemblyConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("assembly", DefaultValue = null, IsRequired = false)]
        public string Assembly
        {
            get { return (string)this["assembly"]; }
            set { this["assembly"] = value; }
        }
    }
}
