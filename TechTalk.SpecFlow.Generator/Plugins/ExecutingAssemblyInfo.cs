using System;
using System.Reflection;

namespace TechTalk.SpecFlow.Generator.Plugins
{
    public interface IExecutingAssemblyInfo
    {
        /// <summary>
        /// Gets the location of the assembly as specified originally, for example, in an System.Reflection.AssemblyName object.
        /// </summary>
        string GetCodeBase();

        /// <summary>
        /// Gets the assembly version.
        /// </summary>
        Version GetVersion();
    }

    public class ExecutingAssemblyInfo : IExecutingAssemblyInfo
    {
        public string GetCodeBase()
        {
            return Assembly.GetExecutingAssembly().CodeBase;
        }

        public Version GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }
    }
}
