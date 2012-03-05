using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace TechTalk.SpecFlow.VsIntegration.Common
{
    [System.Runtime.InteropServices.ComVisible(true)]
    [Guid("8EC3552F-9C6A-472f-8E64-630C7CAE0179")]
    public abstract partial class BaseCodeGenerator : IVsSingleFileGenerator
    {
        private IVsGeneratorProgress codeGeneratorProgress;
        private string codeFileNameSpace = String.Empty;
        private string codeFilePath = String.Empty;

        public string CodeFileNameSpace
        {
            get { return codeFileNameSpace; }
        }

        public string CodeFilePath
        {
            get { return codeFilePath; }
        }

        public IVsGeneratorProgress CodeGeneratorProgress
        {
            get { return codeGeneratorProgress; }
        }

        int IVsSingleFileGenerator.DefaultExtension(
            out string pbstrDefaultExtension)
        {
            pbstrDefaultExtension = GetDefaultExtension();
            return VSConstants.S_OK;
        }

        protected virtual string GetMessage(Exception ex)
        {
            if (ex.InnerException == null)
                return ex.Message;

            return ex.Message + " -> " + GetMessage(ex.InnerException);
        }

        int IVsSingleFileGenerator.Generate(string wszInputFilePath,
                                            string bstrInputFileContents,
                                            string wszDefaultNamespace,
                                            IntPtr[] rgbOutputFileContents,
                                            out uint pcbOutput,
                                            IVsGeneratorProgress pGenerateProgress)
        {
            string trimmedInputFileContents = bstrInputFileContents.Trim();
            codeFilePath = wszInputFilePath;
            codeFileNameSpace = wszDefaultNamespace;
            codeGeneratorProgress = pGenerateProgress;
            byte[] bytes = null;
            try
            {
                BeforeCodeGenerated();
                string generatedCode = null;
                if (IsSharePointFeature(trimmedInputFileContents))
                {
                    StringBuilder sharePointFeatureComment = new StringBuilder();
                    sharePointFeatureComment.AppendFormat("/*SpecFlow tried to generate a test class file, but {0} appears to be a SharePoint feature.", wszInputFilePath);
                    sharePointFeatureComment.AppendLine("  The SpecFlow test class was not be generated in order to avoid errors in the SharePoint proejct*/");
                    generatedCode = sharePointFeatureComment.ToString();
                }
                else
                {
                    generatedCode = GenerateCode(bstrInputFileContents);
                }
                if (generatedCode == null)
                {
                    bytes = GetBytesForError(pGenerateProgress, null);
                }
                else
                {
                    bytes = Encoding.UTF8.GetBytes(generatedCode);
                    AfterCodeGenerated(false);
                }
            }
            catch(Exception ex)
            {
                bytes = GetBytesForError(pGenerateProgress, ex);
            }

            byte[] utf8BOM = new byte[] { 0xEF, 0xBB, 0xBF };
            int outputLength = utf8BOM.Length + bytes.Length;
            byte[] output = new byte[outputLength];
            Array.Copy(utf8BOM, 0, output, 0, utf8BOM.Length);
            Array.Copy(bytes, 0, output, utf8BOM.Length, bytes.Length);
            rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(outputLength);
            Marshal.Copy(output, 0, rgbOutputFileContents[0], outputLength);
            pcbOutput = (uint)outputLength;

            RefreshMsTestWindow();

            return VSConstants.S_OK;
        }

        private bool IsSharePointFeature(string trimmedInputFileContents)
        {
            return trimmedInputFileContents.StartsWith("<?xml version=\"1.0\" encoding=\"utf-8\"?>") &&
                   trimmedInputFileContents.Contains("<feature ") &&
                   trimmedInputFileContents.Contains("$SharePoint.Project.FileNameWithoutExtension$_$SharePoint.Feature.FileNameWithoutExtension$");
        }

        private byte[] GetBytesForError(IVsGeneratorProgress pGenerateProgress, Exception ex)
        {
            byte[] bytes;
            string message = GenerateError(pGenerateProgress, ex);
            AfterCodeGenerated(true);
            bytes = Encoding.UTF8.GetBytes(GetGeneratedCodeForFailure(message));
            return bytes;
        }

        protected virtual string GetGeneratedCodeForFailure(string errorMessage)
        {
            return errorMessage;
        }

        protected virtual void RefreshMsTestWindow()
        {
            //refreshCmdGuid,cmdID is the command id of refresh command.
            Guid refreshCmdGuid = new Guid("{B85579AA-8BE0-4C4F-A850-90902B317571}");
            IOleCommandTarget cmdTarget = Package.GetGlobalService(typeof(SUIHostCommandDispatcher)) as IOleCommandTarget;
            const uint cmdID = 13109;
            if (cmdTarget != null)
                cmdTarget.Exec(ref refreshCmdGuid, cmdID, 
                    (uint)OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER, 
                    IntPtr.Zero, IntPtr.Zero);
        }

        protected virtual string GenerateError(IVsGeneratorProgress pGenerateProgress, Exception ex)
        {
            string message = GetMessage(ex);
            pGenerateProgress.GeneratorError(0, 4, message, 0xFFFFFFFF, 0xFFFFFFFF);
            return message;
        }

        protected virtual void BeforeCodeGenerated()
        {
            //nop
        }

        protected virtual void AfterCodeGenerated(bool error)
        {
            //nop
        }

        protected abstract string GetDefaultExtension();
        protected abstract string GenerateCode(string inputFileContent);
    }
}