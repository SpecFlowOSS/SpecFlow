using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TechTalk.SpecFlow.Configuration
{
    /// <summary>
    /// Represents the subset of runtime configuration that might be interesing for the tooling
    /// </summary>
    public class RuntimeConfigurationForGenerator
    {
        private readonly List<string> additionalStepAssemblies = new List<string>();

        public IEnumerable<string> AdditionalStepAssemblies
        {
            get
            {
                return additionalStepAssemblies;
            }
        }

        internal RuntimeConfigurationForGenerator UpdateFromConfigFile(ConfigurationSectionHandler configSection)
        {
            if (configSection == null) throw new ArgumentNullException("configSection");

            var config = this;

            foreach (var element in configSection.StepAssemblies)
            {
                var stepAssembly = ((StepAssemblyConfigElement)element).Assembly;
                config.additionalStepAssemblies.Add(stepAssembly);
            }

            return config;
        }

        #region Equality

        public bool Equals(RuntimeConfigurationForGenerator other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.additionalStepAssemblies.SequenceEqual(additionalStepAssemblies);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (RuntimeConfigurationForGenerator)) return false;
            return Equals((RuntimeConfigurationForGenerator) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = additionalStepAssemblies.Count.GetHashCode();
                return result;
            }
        }

        #endregion
    }
}