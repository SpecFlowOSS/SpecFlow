using System.Collections.Generic;
using System.Reflection;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.RuntimeTests.Infrastructure
{
    internal static class RuntimeConfigurationExtensions
    {
        public static void AddAdditionalStepAssembly(this RuntimeConfiguration runtimeConfiguration, Assembly assembly)
        {
            runtimeConfiguration.AdditionalStepAssemblies.Add(assembly.FullName);
        }
    }
}