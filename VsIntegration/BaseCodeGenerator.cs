using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace TechTalk.SpecFlow.VsIntegration
{
    [System.Runtime.InteropServices.ComVisible(true)]
    [Guid("8EC3552F-9C6A-472f-8E64-630C7CAE0179")]
    public abstract class BaseCodeGenerator : IVsSingleFileGenerator
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
            codeFilePath = wszInputFilePath;
            codeFileNameSpace = wszDefaultNamespace;
            codeGeneratorProgress = pGenerateProgress;
            byte[] bytes = null;
            try
            {
                BeforeCodeGenerated();
                bytes = Encoding.UTF8.GetBytes(GenerateCode(bstrInputFileContents));
                AfterCodeGenerated(false);
            }
            catch(Exception ex)
            {
                string message = GenerateError(pGenerateProgress, ex);
                AfterCodeGenerated(true);
                bytes = Encoding.UTF8.GetBytes(message);
            }

//            if (bytes == null)
//            {
//                // --- This signals that GenerateCode() has failed.         
//                rgbOutputFileContents = null;
//                pcbOutput = 0;
//                return VSConstants.E_FAIL;
//            }

            int outputLength = bytes.Length;
            rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(outputLength);
            Marshal.Copy(bytes, 0, rgbOutputFileContents[0], outputLength);
            pcbOutput = (uint)outputLength;
            return VSConstants.S_OK;
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