using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Vs2010Integration;

namespace TechTalk.SpecFlow.VsIntegration.Common
{
    [System.Runtime.InteropServices.ComVisible(true)]
    [Guid("AC7359F9-6206-47A3-935C-EF191D73B9ED")]
    public abstract partial class SpecFlowSingleFileGeneratorBase : BaseCodeGeneratorWithSite
    {
        protected override string GetDefaultExtension()
        {
            CodeDomProvider provider = GetCodeProvider();

            return ".feature." + provider.FileExtension;
        }

        protected override string GenerateError(Microsoft.VisualStudio.Shell.Interop.IVsGeneratorProgress pGenerateProgress, Exception ex)
        {
            if (ex is SpecFlowParserException)
            {
                SpecFlowParserException sfpex = (SpecFlowParserException) ex;
                if (sfpex.ErrorDetails == null || sfpex.ErrorDetails.Count == 0)
                    return base.GenerateError(pGenerateProgress, ex);

                foreach (var errorDetail in sfpex.ErrorDetails)
                    pGenerateProgress.GeneratorError(0, 4, errorDetail.Message, 
                        (uint)errorDetail.ForcedLine - 1,
                        (uint)errorDetail.ForcedColumn - 1);

                return GetMessage(ex);
            }

            return base.GenerateError(pGenerateProgress, ex);
        }
    }
}
