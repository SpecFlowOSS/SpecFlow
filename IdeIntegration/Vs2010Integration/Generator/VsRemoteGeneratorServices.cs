using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using EnvDTE;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.Generator
{
    internal abstract class VsRemoteGeneratorServices : RemoteGeneratorServices
    {
        protected readonly Project project;

        protected VsRemoteGeneratorServices(ITestGeneratorFactory testGeneratorFactory, IRemoteAppDomainTestGeneratorFactory remoteAppDomainTestGeneratorFactory, bool enableSettingsCache, Project project, IVisualStudioTracer visualStudioTracer)
            : base(testGeneratorFactory, remoteAppDomainTestGeneratorFactory, visualStudioTracer, enableSettingsCache)
        {
            this.project = project;
        }

        private static readonly string[] probingPaths = new[]
                                                            {
                                                                @".",
                                                                @"..\tools",
                                                                @"..\..\tools",
                                                            };
        const string generatorAssemblyName = "TechTalk.SpecFlow.Generator";

        protected override GeneratorInfo GetGeneratorInfo()
        {
            tracer.Trace("Discovering generator information...", "VsRemoteGeneratorServices");
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
                tracer.Trace(exception.ToString(), "VsRemoteGeneratorServices");
                return null;
            }
        }

        private GeneratorInfo DetectFromConfig()
        {
            try
            {
                var config = new RuntimeSpecFlowProjectConfigurationLoader()
                    .LoadConfiguration(GetProjectSettings().ConfigurationHolder, new VsProjectReference(project));
                if (config == null || string.IsNullOrWhiteSpace(config.GeneratorConfiguration.GeneratorPath))
                    return null;

                var generatorFolder = Path.GetFullPath(
                    Path.Combine(VsxHelper.GetProjectFolder(project), config.GeneratorConfiguration.GeneratorPath));
    
                tracer.Trace("Generator is configured to be at " + generatorFolder, "VsRemoteGeneratorServices");
                return DetectFromFolder(generatorFolder);
            }
            catch(Exception exception)
            {
                tracer.Trace(exception.ToString(), "VsRemoteGeneratorServices");
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

            tracer.Trace("Runtime found at " + runtimeFolder, "VsRemoteGeneratorServices");

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

            tracer.Trace("Generator found at " + generatorPath, "VsRemoteGeneratorServices");
            var fileVersion = FileVersionInfo.GetVersionInfo(generatorPath);
            if (fileVersion.FileVersion == null)
            {
                tracer.Trace("Could not detect generator version", "VsRemoteGeneratorServices");
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

            tracer.Trace("Direct generator reference found", "VsRemoteGeneratorServices");
            return new GeneratorInfo
                       {
                           GeneratorAssemblyVersion = new Version(specFlowGeneratorRef.Version),
                           GeneratorFolder = Path.GetDirectoryName(specFlowGeneratorRef.Path)
                       };
        }
    }
}