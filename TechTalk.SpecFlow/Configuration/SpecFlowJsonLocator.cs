using System;
using System.IO;

namespace TechTalk.SpecFlow.Configuration
{
    public class SpecFlowJsonLocator : ISpecFlowJsonLocator
    {
        private readonly SpecFlowConfigFileNameProvider _configFileNameProvider;

        public SpecFlowJsonLocator(SpecFlowConfigFileNameProvider configFileNameProvider)
        {
            _configFileNameProvider = configFileNameProvider;
        }

        public string GetSpecFlowJsonFilePath()
        {
            string jsonFileName = _configFileNameProvider.ConfigFileName;

            var specflowJsonFileInAppDomainBaseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, jsonFileName);

            if (File.Exists(specflowJsonFileInAppDomainBaseDirectory))
            {
                return specflowJsonFileInAppDomainBaseDirectory;
            }

            var specflowJsonFileTwoDirectoriesUp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", jsonFileName);

            if (File.Exists(specflowJsonFileTwoDirectoriesUp))
            {
                return specflowJsonFileTwoDirectoriesUp;
            }

            var specflowJsonFileInCurrentDirectory = Path.Combine(Environment.CurrentDirectory, jsonFileName);

            if (File.Exists(specflowJsonFileInCurrentDirectory))
            {
                return specflowJsonFileInCurrentDirectory;
            }

            return null;
        }
    }
}
