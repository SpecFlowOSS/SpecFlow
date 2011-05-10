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
            try
            {
                GeneratorInfo generatorInfo = null;

                generatorInfo = DetectFromConfig();

                if (generatorInfo == null)
                    generatorInfo = DetectDirectGeneratorReference();

                if (generatorInfo == null)
                    generatorInfo = DetectFromRuntimeReference();

                return generatorInfo;
            }
            catch (Exception exception)
            {
                tracer.Trace(exception.ToString(), "VsGeneratorInfoProvider");
                return null;
            }
        }

        private GeneratorInfo DetectFromConfig()
        {
            try
            {
                //TODO: have a "project context" where the actual confic can be read without re-loading/parsing it.
                var configurationHolder = configurationReader.ReadConfiguration();
                var config = new SpecFlowProjectConfigurationLoaderWithoutPlugins().LoadConfiguration(configurationHolder);
                if (config == null || string.IsNullOrWhiteSpace(config.GeneratorConfiguration.GeneratorPath))
                    return null;

                var generatorFolder = Path.GetFullPath(
                    Path.Combine(VsxHelper.GetProjectFolder(project), config.GeneratorConfiguration.GeneratorPath));

                tracer.Trace("Generator is configured to be at " + generatorFolder, "VsGeneratorInfoProvider");
                return DetectFromFolder(generatorFolder);
            }
            catch(Exception exception)
            {
                tracer.Trace(exception.ToString(), "VsGeneratorInfoProvider");
                return null;
            }
        }

        private GeneratorInfo DetectFromRuntimeReference()
        {
            var specFlowRef = VsxHelper.GetReference(project, "TechTalk.SpecFlow");
            if (specFlowRef == null)
                return null;

            if (specFlowRef.Path == null)
                return null;

            string runtimeFolder = Path.GetDirectoryName(specFlowRef.Path);
            if (runtimeFolder == null)
                return null;

            tracer.Trace("Runtime found at " + runtimeFolder, "VsGeneratorInfoProvider");

            return probingPaths.Select(probingPath => Path.GetFullPath(Path.Combine(runtimeFolder, probingPath)))
                .Select(DetectFromFolder)
                .FirstOrDefault(generatorInfo => generatorInfo != null);
        }

        private GeneratorInfo DetectFromFolder(string generatorFolder)
        {
            const string generatorAssemblyFileName = generatorAssemblyName + ".dll";

            var generatorPath = Path.Combine(generatorFolder, generatorAssemblyFileName);
            if (!File.Exists(generatorPath))
                return null;

            tracer.Trace("Generator found at " + generatorPath, "VsGeneratorInfoProvider");
            var fileVersion = FileVersionInfo.GetVersionInfo(generatorPath);
            if (fileVersion.FileVersion == null)
            {
                tracer.Trace("Could not detect generator version", "VsGeneratorInfoProvider");
                return null;
            }

            return new GeneratorInfo
                       {
                           GeneratorAssemblyVersion = new Version(fileVersion.FileVersion),
                           GeneratorFolder = Path.GetDirectoryName(generatorPath)
                       };
        }

        private GeneratorInfo DetectDirectGeneratorReference()
        {
            var specFlowGeneratorRef = VsxHelper.GetReference(project, generatorAssemblyName);
            if (specFlowGeneratorRef == null)
                return null;

            tracer.Trace("Direct generator reference found", "VsGeneratorInfoProvider");
            return new GeneratorInfo
                       {
                           GeneratorAssemblyVersion = new Version(specFlowGeneratorRef.Version),
                           GeneratorFolder = Path.GetDirectoryName(specFlowGeneratorRef.Path)
                       };
        }
    }
}