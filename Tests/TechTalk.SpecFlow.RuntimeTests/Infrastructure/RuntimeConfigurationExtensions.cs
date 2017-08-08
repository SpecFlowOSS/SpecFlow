using System.Collections.Generic;
using System.Reflection;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.RuntimeTests.Infrastructure
{
    internal static class RuntimeConfigurationExtensions
    {
        public static void AddAdditionalStepAssembly(this SpecFlow.Configuration.SpecFlowConfiguration specFlowConfiguration, Assembly assembly)
        {
            specFlowConfiguration.AdditionalStepAssemblies.Add(assembly.FullName);
        }
    }
}