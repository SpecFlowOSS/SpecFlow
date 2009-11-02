using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml;

namespace TechTalk.SpecFlow.Configuration
{
    public enum MissingOrPendingStepsOutcome
    {
        Inconclusive,
        Ignore,
        Error
    }

    public static class ConfigDefaults
    {
        internal const string FeatureLanguage = "en";
        internal const string ToolLanguage = "";

        internal const string UnitTestProviderName = "NUnit";

        internal const bool DetectAmbiguousMatches = true;
        internal const bool StopAtFirstError = false;
        internal const MissingOrPendingStepsOutcome MissingOrPendingStepsOutcome = TechTalk.SpecFlow.Configuration.MissingOrPendingStepsOutcome.Inconclusive;

        internal const bool TraceSuccessfulSteps = true;
        internal const bool TraceTimings = false;
        internal const string MinTracedDuration = "0:0:0.1";
    }

    partial class ConfigurationSectionHandler : ConfigurationSection
    {
        [ConfigurationProperty("globalization", IsRequired = false)]
        public GlobalizationConfigElement Globalization
        {
            get { return (GlobalizationConfigElement)this["globalization"]; }
            set { this["globalization"] = value; }
        }

        [ConfigurationProperty("unitTestProvider", IsRequired = false)]
        public UnitTestProviderConfigElement UnitTestProvider
        {
            get { return (UnitTestProviderConfigElement)this["unitTestProvider"]; }
            set { this["unitTestProvider"] = value; }
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

        static internal ConfigurationSectionHandler CreateFromXml(string xmlContent)
        {
            ConfigurationSectionHandler section = new ConfigurationSectionHandler();
            section.Init();
            section.Reset(null);
            using (var reader = new XmlTextReader(new StringReader(xmlContent)))
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

    public class GlobalizationConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("language", DefaultValue = "en", IsRequired = false)]
        [RegexStringValidator(@"\w{2}(-\w{2})?")]
        public string Language 
        {
            get { return (String)this["language"]; }
            set { this["language"] = value; }
        }

        [ConfigurationProperty("toolLanguage", DefaultValue = "", IsRequired = false)]
        [RegexStringValidator(@"\w{2}(-\w{2})?|")]
        public string ToolLanguage
        {
            get { return (String)this["toolLanguage"]; }
            set { this["toolLanguage"] = value; }
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
}
