using System;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.VsIntegration.SingleFileGenerator;
using VSLangProj80;

namespace TechTalk.SpecFlow.Vs2008Integration
{
    [ComVisible(true)]
    [Guid("3C9CF10A-A9AB-4899-A0FB-4B3BE4A36C15")]
    [CodeGeneratorRegistration(typeof(SpecFlowSingleFileGenerator), "C# XML Class Generator", vsContextGuids.vsContextGuidVCSProject, GeneratesDesignTimeSource = true)]
    [CodeGeneratorRegistration(typeof(SpecFlowSingleFileGenerator), "VB XML Class Generator", vsContextGuids.vsContextGuidVBProject, GeneratesDesignTimeSource = true)]
    [ProvideObject(typeof(SpecFlowSingleFileGenerator))]
    public class SpecFlowSingleFileGenerator : SpecFlowSingleFileGeneratorBase
    {
        protected override Func<GeneratorServices> GeneratorServicesProvider(Project project)
        {
            return () => new Vs2008GeneratorServices(project);
        }
    }
}
