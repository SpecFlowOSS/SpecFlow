using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using EnvDTE;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.Generator
{
    internal class VsGeneratorInfoProvider : IGeneratorInfoProvider
    {
        private static readonly string[] probingPaths = new[]
                                                            {
                                                                @".",
                                                                @"tools",
                                                                @"..\tools",
                                                                @"..\..\tools",
                                                            };
        const string generatorAssemblyName = "TechTalk.SpecFlow.Generator";

        protected readonly Project project;
        protected readonly IIdeTracer tracer;
        private readonly ISpecFlowConfigurationReader configurationReader;

        public VsGeneratorInfoProvider(Project project, IIdeTracer tracer, ISpecFlowConfigurationReader configurationReader)
        {
            this.project = project;
            this.tracer = tracer;
            this.configurationReader = configurationReader;
        }

        public virtual GeneratorInfo GetGeneratorInfo()
        {
            tracer.Trace("Discovering generator information...", "VsGeneratorInfoProvider");

            GeneratorConfiguration generatorConfiguration = GenGeneratorConfig();

            try
            {
                var generatorInfo = new GeneratorInfo
                                        {
                                            UsesPlugins = generatorConfiguration.UsesPlugins
                                        };

                if (DetectFromConfig(generatorInfo, generatorConfiguration))
                    return generatorInfo;

                if (DetectDirectGeneratorReference(generatorInfo))
                    return generatorInfo;

                if (!DetectFromRuntimeReference(generatorInfo))
                    tracer.Trace("Unable to detect generator path", "VsGeneratorInfoProvider");
                
                return generatorInfo;
            }
            catch (Exception exception)
            {
                tracer.Trace(exception.ToString(), "VsGeneratorInfoProvider");
                return null;
            }
        }

        private GeneratorConfiguration GenGeneratorConfig()
        {
            try
            {
                //TODO: have a "project context" where the actual confic can be read without re-loading/parsing it.
                var configurationHolder = configurationReader.ReadConfiguration();
                var config = new GeneratorConfigurationProvider().LoadConfiguration(configurationHolder);
                if (config == null)
                    return new GeneratorConfiguration();

                return config.GeneratorConfiguration;
            }
            catch (Exception exception)
            {
                tracer.Trace("Config load error: " + exception, "VsGeneratorInfoProvider");
                return new GeneratorConfiguration();
            }
        }

        private bool DetectFromConfig(GeneratorInfo generatorInfo, GeneratorConfiguration generatorConfiguration)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(generatorConfiguration.GeneratorPath))
                    return false;

                var generatorFolder = Path.GetFullPath(
                    Path.Combine(VsxHelper.GetProjectFolder(project), generatorConfiguration.GeneratorPath));

                tracer.Trace("Generator is configured to be at " + generatorFolder, "VsGeneratorInfoProvider");
                return DetectFromFolder(generatorInfo, generatorFolder);
            }
            catch(Exception exception)
            {
                tracer.Trace(exception.ToString(), "VsGeneratorInfoProvider");
                return false;
            }
        }

        private bool DetectFromRuntimeReference(GeneratorInfo generatorInfo)
        {
            var specFlowRef = VsxHelper.GetReference(project, "TechTalk.SpecFlow");
            if (specFlowRef == null)
                return false;

            var specFlowRefPath = specFlowRef.Path;
            if (string.IsNullOrWhiteSpace(specFlowRefPath))
                return false;

            string runtimeFolder = Path.GetDirectoryName(specFlowRefPath);
            if (runtimeFolder == null)
                return false;

            tracer.Trace("Runtime found at " + runtimeFolder, "VsGeneratorInfoProvider");

            return probingPaths.Select(probingPath => Path.GetFullPath(Path.Combine(runtimeFolder, probingPath)))
                .Any(probingPath => DetectFromFolder(generatorInfo, probingPath));
        }

        private bool DetectFromFolder(GeneratorInfo generatorInfo, string generatorFolder)
        {
            const string generatorAssemblyFileName = generatorAssemblyName + ".dll";

            var generatorPath = Path.Combine(generatorFolder, generatorAssemblyFileName);
            if (!File.Exists(generatorPath))
                return false;

            tracer.Trace("Generator found at " + generatorPath, "VsGeneratorInfoProvider");
            var fileVersion = FileVersionInfo.GetVersionInfo(generatorPath);
            if (fileVersion.FileVersion == null)
            {
                tracer.Trace("Could not detect generator version", "VsGeneratorInfoProvider");
                return false;
            }

            generatorInfo.GeneratorAssemblyVersion = new Version(fileVersion.FileVersion);
            generatorInfo.GeneratorFolder = Path.GetDirectoryName(generatorPath);

            return true;
        }

        private bool DetectDirectGeneratorReference(GeneratorInfo generatorInfo)
        {
            var specFlowGeneratorRef = VsxHelper.GetReference(project, generatorAssemblyName);
            if (specFlowGeneratorRef == null)
                return false;

            tracer.Trace("Direct generator reference found", "VsGeneratorInfoProvider");
            generatorInfo.GeneratorAssemblyVersion = new Version(specFlowGeneratorRef.Version);
            generatorInfo.GeneratorFolder = Path.GetDirectoryName(specFlowGeneratorRef.Path);

            return true;
        }
    }
}