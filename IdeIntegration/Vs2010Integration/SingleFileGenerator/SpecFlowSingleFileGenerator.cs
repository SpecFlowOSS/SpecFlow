using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Utils;
using TechTalk.SpecFlow.Vs2010Integration.Generator;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.Utils;
using TechTalk.SpecFlow.VsIntegration.Common;
using VSLangProj80;
using System.Linq;

namespace TechTalk.SpecFlow.Vs2010Integration
{
    [ComVisible(true)]
    [Guid("44F8C2E2-18A9-4B97-B830-6BCD0CAA161C")]
    [CodeGeneratorRegistration(typeof(SpecFlowSingleFileGenerator), "C# SpecFlow Generator", vsContextGuids.vsContextGuidVCSProject, GeneratesDesignTimeSource = true, FileExtension = ".feature")]
    [CodeGeneratorRegistration(typeof(SpecFlowSingleFileGenerator), "VB.NET SpecFlow Generator", vsContextGuids.vsContextGuidVBProject, GeneratesDesignTimeSource = true, FileExtension = ".feature")]
    [CodeGeneratorRegistration(typeof(SpecFlowSingleFileGenerator), "Silverlight SpecFlow Generator", GuidList.vsContextGuidSilverlightProject, GeneratesDesignTimeSource = true, FileExtension = ".feature")]
    [ProvideObject(typeof(SpecFlowSingleFileGenerator))]
    public class SpecFlowSingleFileGenerator : BaseCodeGeneratorWithSite
    {
        protected override string GetDefaultExtension()
        {
            CodeDomProvider provider = GetCodeProvider();

            return ".feature." + provider.FileExtension;
        }

        protected override void RefreshMsTestWindow()
        {
            //the automatic refresh of the test window causes problems in VS2010 -> skip
        }

        private TestGeneratorResult generationResult = null;

        protected override string GenerateCode(string inputFileContent)
        {
            var generatorServices = new VsGeneratorServices(CurrentProject);

            using (var testGenerator = generatorServices.CreateTestGenerator())
            {
                string projectFolder = VsxHelper.GetProjectFolder(CurrentProject);
                var fullPath = Path.GetFullPath(Path.Combine(projectFolder, CodeFilePath));
                FeatureFileInput featureFileInput =
                    new FeatureFileInput(FileSystemHelper.GetRelativePath(fullPath, projectFolder))
                        {
                            FeatureFileContent = inputFileContent
                        };
                generationResult = testGenerator.GenerateTestFile(featureFileInput, new GenerationSettings());
                if (!generationResult.Success)
                    return null;

                return generationResult.GeneratedTestCode;
            }
        }

        protected override string GenerateError(IVsGeneratorProgress pGenerateProgress, Exception ex)
        {
            if (ex == null)
            {
                VisualStudioTracer.Assert(generationResult != null, "no reneration result found");
                VisualStudioTracer.Assert(!generationResult.Success, "generation result is not a failure");

                foreach (var error in generationResult.Errors)
                {
                    pGenerateProgress.GeneratorError(0, 4, error.Message, (uint) error.Line, (uint) error.LinePosition);
                }

                return string.Join(Environment.NewLine, generationResult.Errors.Select(e => e.Message));
            }
            return base.GenerateError(pGenerateProgress, ex);
        }
    }
}
