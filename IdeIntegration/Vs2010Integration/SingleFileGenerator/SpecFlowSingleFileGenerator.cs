using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using TechTalk.SpecFlow.VsIntegration.Common;
using VSLangProj80;

namespace TechTalk.SpecFlow.Vs2010Integration
{
    [System.Runtime.InteropServices.ComVisible(true)]
    [Guid("44F8C2E2-18A9-4B97-B830-6BCD0CAA161C")]
    [CodeGeneratorRegistration(typeof(SpecFlowSingleFileGenerator),
      "C# XML Class Generator", vsContextGuids.vsContextGuidVCSProject,
      GeneratesDesignTimeSource = true)]
    [CodeGeneratorRegistration(typeof(SpecFlowSingleFileGenerator),
      "VB XML Class Generator", vsContextGuids.vsContextGuidVBProject,
      GeneratesDesignTimeSource = true)]
    [ProvideObject(typeof(SpecFlowSingleFileGenerator))]
    public class SpecFlowSingleFileGenerator : SpecFlowSingleFileGeneratorBase
    {
        protected override void RefreshMsTestWindow()
        {
            //the automatic refresh of the test window causes problems in VS2010 -> skip
        }
    }
}
