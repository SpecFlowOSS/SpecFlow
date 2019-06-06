using System.IO;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.TestFramework;

namespace TechTalk.SpecFlow.NUnit.SpecFlowPlugin
{
    public class NUnitNetFrameworkTestRunContext : ITestRunContext
    {
        private readonly ISpecFlowPath _specFlowPath;

        public NUnitNetFrameworkTestRunContext(ISpecFlowPath specFlowPath)
        {
            _specFlowPath = specFlowPath;
        }

        public string GetTestDirectory() => Path.GetDirectoryName(_specFlowPath.GetPathToSpecFlowDll());
    }
}
