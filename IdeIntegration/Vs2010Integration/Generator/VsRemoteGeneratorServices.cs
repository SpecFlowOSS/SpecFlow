using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using EnvDTE;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.Generator
{
    internal abstract class VsRemoteGeneratorServices : RemoteGeneratorServices
    {
        protected readonly Project project;

        protected VsRemoteGeneratorServices(ITestGeneratorFactory testGeneratorFactory, bool enableSettingsCache, Project project, IVisualStudioTracer visualStudioTracer)
            : base(testGeneratorFactory, visualStudioTracer, enableSettingsCache)
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

                //TODO: load generator path from configuration

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

            const string generatorAssemblyFileName = generatorAssemblyName + ".dll";
            foreach (var probingPath in probingPaths)
            {
                var generatorPath = Path.GetFullPath(Path.Combine(runtimeFolder, probingPath, generatorAssemblyFileName));
                if (File.Exists(generatorPath))
                {
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
            }

            return null;
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