using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using Microsoft.VisualStudio.Designer.Interfaces;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using TechTalk.SpecFlow.Parser;
using VSLangProj80;

namespace TechTalk.SpecFlow.VsIntegration
{
    [System.Runtime.InteropServices.ComVisible(true)]
    [Guid("3C9CF10A-A9AB-4899-A0FB-4B3BE4A36C15")]
    [CodeGeneratorRegistration(typeof(SpecFlowSingleFileGenerator),
      "C# XML Class Generator", vsContextGuids.vsContextGuidVCSProject,
      GeneratesDesignTimeSource = true)]
    [CodeGeneratorRegistration(typeof(SpecFlowSingleFileGenerator),
      "VB XML Class Generator", vsContextGuids.vsContextGuidVBProject,
      GeneratesDesignTimeSource = true)]
    [CodeGeneratorRegistration(typeof(SpecFlowSingleFileGenerator),
      "J# XML Class Generator", vsContextGuids.vsContextGuidVJSProject,
      GeneratesDesignTimeSource = true)]
    [ProvideObject(typeof(SpecFlowSingleFileGenerator))]
    public class SpecFlowSingleFileGenerator : BaseCodeGeneratorWithSite
    {
        protected override string GetDefaultExtension()
        {
            return ".feature.cs";
        }

        protected override string GenerateCode(string inputFileContent)
        {
            CodeDomProvider provider = GetCodeProvider();

            CodeCompileUnit compileUnit = SpecFlowTestGenerator.GenerateTestFile(inputFileContent, CodeFilePath, CodeFileNameSpace);
            using (StringWriter writer = new StringWriter(new StringBuilder()))
            {
                CodeGeneratorOptions options = new CodeGeneratorOptions();
                options.BracingStyle = "C";
                provider.GenerateCodeFromCompileUnit(compileUnit, writer,
                                                     options);
                writer.Flush();

                return writer.ToString();
            }
        }

        protected override string GenerateError(Microsoft.VisualStudio.Shell.Interop.IVsGeneratorProgress pGenerateProgress, Exception ex)
        {
            if (ex is SpecFlowParserException)
            {
                SpecFlowParserException sfpex = (SpecFlowParserException) ex;
                if (sfpex.DetailedErrors == null)
                    return base.GenerateError(pGenerateProgress, ex);

                Regex coordRe = new Regex(@"^\((?<line>\d+),(?<col>\d+)\)");
                var match = coordRe.Match(sfpex.DetailedErrors[0]);
                if (!match.Success)
                    return base.GenerateError(pGenerateProgress, ex);

                string message = GetMessage(ex);
                pGenerateProgress.GeneratorError(0, 4, message,
                    uint.Parse(match.Groups["line"].Value) - 1, 
                    uint.Parse(match.Groups["col"].Value));
                return message;
            }

            return base.GenerateError(pGenerateProgress, ex);
        }

//        protected override string GetMessage(Exception ex)
//        {
//            if (ex is SpecFlowParserException)
//            {
//                SpecFlowParserException sfpex = (SpecFlowParserException)ex;
//                if (sfpex.DetailedErrors == null)
//                    return base.GetMessage(ex);
//
//                return string.Join(
//                    Environment.NewLine,
//                    sfpex.DetailedErrors.Select(e => CodeFilePath + e).ToArray());
//            }
//            return base.GetMessage(ex);
//        }
    }
}