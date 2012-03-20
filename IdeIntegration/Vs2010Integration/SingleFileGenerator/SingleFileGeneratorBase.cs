using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Designer.Interfaces;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSOLE = Microsoft.VisualStudio.OLE.Interop;

namespace TechTalk.SpecFlow.VsIntegration.SingleFileGenerator
{
    public class SingleFileGeneratorError
    {
        /// <summary>
        /// Zero-based line index.
        /// </summary>
        public readonly int Line;
        /// <summary>
        /// Zero-based position index.
        /// </summary>
        public readonly int LinePosition;
        public readonly string Message;

        public SingleFileGeneratorError(string message) : this(0, 0, message)
        {
        }

        public SingleFileGeneratorError(int line, int linePosition, string message)
        {
            Line = line;
            LinePosition = linePosition;
            Message = message;
        }
    }

    [ComVisible(true)]
    [Guid("9AEFA7A0-BC60-447E-B434-0DE2FD75693A")]
    public abstract class SingleFileGeneratorBase: IVsSingleFileGenerator, VSOLE.IObjectWithSite
    {
        private readonly string baseExtension;

        protected SingleFileGeneratorBase(string baseExtension)
        {
            this.baseExtension = baseExtension;
        }

        protected virtual string GetDefaultExtension()
        {
            CodeDomProvider provider = GetCodeProvider();
            return baseExtension + "." + provider.FileExtension;
        }

        protected virtual void BeforeCodeGenerated()
        {
            //nop
        }

        protected virtual void AfterCodeGenerated(bool error)
        {
            if (error)
            {
                IVsErrorList errorList = ErrorList;
                if (errorList != null)
                {
                    errorList.BringToFront();
                    errorList.ForceShowErrors();
                }
            }
        }

        protected abstract bool GenerateInternal(string inputFilePath, string inputFileContent, Project project, string defaultNamespace, Action<SingleFileGeneratorError> onError, out string generatedContent);

        protected virtual CodeDomProvider GetCodeProvider()
        {
            if (codeDomProvider == null)
            {
                IVSMDCodeDomProvider provider = GetService(typeof(SVSMDCodeDomProvider))
                  as IVSMDCodeDomProvider;
                if (provider != null)
                {
                    codeDomProvider = provider.CodeDomProvider as CodeDomProvider;
                }
                else
                {
                    codeDomProvider = CodeDomProvider.CreateProvider("C#");
                }
            }
            return codeDomProvider;
        }

        protected IVsErrorList ErrorList
        {
            get
            {
                IVsErrorList objectForIUnknown = null;
                IVsHierarchy service = this.GetService(typeof(IVsHierarchy)) as IVsHierarchy;
                if (service != null)
                {
                    VSOLE.IServiceProvider ppSP;
                    if (!Failed(service.GetSite(out ppSP)) && (ppSP != null))
                    {
                        Guid gUID = typeof(SVsErrorList).GUID;
                        Guid riid = typeof(IVsErrorList).GUID;
                        IntPtr zero;
                        ErrorHandler.ThrowOnFailure(ppSP.QueryService(ref gUID, ref riid, out zero));
                        if (zero != IntPtr.Zero)
                        {
                            objectForIUnknown = Marshal.GetObjectForIUnknown(zero) as IVsErrorList;
                        }
                    }
                }
                return objectForIUnknown;
            }
        }

        private static bool Failed(int hr)
        {
            return (hr < 0);
        }

        protected DTE Dte
        {
            get
            {
                DTE objectForIUnknown = null;
                IVsHierarchy service = this.GetService(typeof(IVsHierarchy)) as IVsHierarchy;
                if (service != null)
                {
                    VSOLE.IServiceProvider ppSP;
                    if (!Failed(service.GetSite(out ppSP)) && (ppSP != null))
                    {
                        Guid gUID = typeof(DTE).GUID;
                        IntPtr zero;
                        ErrorHandler.ThrowOnFailure(ppSP.QueryService(ref gUID, ref gUID, out zero));
                        if (zero != IntPtr.Zero)
                        {
                            objectForIUnknown = Marshal.GetObjectForIUnknown(zero) as DTE;
                        }
                    }
                }
                return objectForIUnknown;
            }
        }

        private ServiceProvider SiteServiceProvider
        {
            get { return serviceProvider ?? (serviceProvider = new ServiceProvider(site as VSOLE.IServiceProvider)); }
        }

        protected object GetService(Type serviceType)
        {
            return SiteServiceProvider.GetService(serviceType);
        }

        protected virtual void RefreshMsTestWindow()
        {
            //refreshCmdGuid,cmdID is the command id of refresh command.
            Guid refreshCmdGuid = new Guid("{B85579AA-8BE0-4C4F-A850-90902B317571}");
            VSOLE.IOleCommandTarget cmdTarget = Package.GetGlobalService(typeof(SUIHostCommandDispatcher)) as VSOLE.IOleCommandTarget;
            const uint cmdID = 13109;
            if (cmdTarget != null)
                cmdTarget.Exec(ref refreshCmdGuid, cmdID, (uint)VSOLE.OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER, IntPtr.Zero, IntPtr.Zero);
        }

        #region Implementation of IVsSingleFileGenerator

        public int DefaultExtension(out string pbstrDefaultExtension)
        {
            pbstrDefaultExtension = GetDefaultExtension();
            return VSConstants.S_OK;
        }

        public int Generate(string inputFilePath, string inputFileContents, string defaultNamespace, IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress generateProgress)
        {
            BeforeCodeGenerated();

            string generatedContent;
            Action<SingleFileGeneratorError> onError = 
                delegate(SingleFileGeneratorError error)
                    {
                        generateProgress.GeneratorError(0, 4, error.Message, (uint)(error.Line + 1), (uint)(error.LinePosition + 1));
                    };

            Project project = GetProjectForSourceFile(Dte, inputFilePath);
            bool successfulGeneration = false;
            if (project == null)
            {
                onError(new SingleFileGeneratorError("Unable to detect current project."));
                generatedContent = "";
            }
            else
            {
                successfulGeneration = GenerateInternal(inputFilePath, inputFileContents, project, defaultNamespace, onError, out generatedContent);
            }

            byte[] bytes = Encoding.UTF8.GetBytes(generatedContent);

            byte[] utf8BOM = new byte[] { 0xEF, 0xBB, 0xBF };
            int outputLength = utf8BOM.Length + bytes.Length;
            byte[] output = new byte[outputLength];
            Array.Copy(utf8BOM, 0, output, 0, utf8BOM.Length);
            Array.Copy(bytes, 0, output, utf8BOM.Length, bytes.Length);
            rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(outputLength);
            Marshal.Copy(output, 0, rgbOutputFileContents[0], outputLength);
            pcbOutput = (uint)outputLength;

            AfterCodeGenerated(!successfulGeneration);

            return VSConstants.S_OK;
        }

        private Project GetProjectForSourceFile(DTE dte, string filePath)
        {
            if (dte != null)
            {
                ProjectItem prjItem = dte.Solution.FindProjectItem(filePath);
                if (prjItem != null)
                    return prjItem.ContainingProject;
            }
            throw new InvalidOperationException("Unable to detect current project.");
        }

        #endregion

        #region Implementation of IObjectWithSite

        private object site = null;
        private ServiceProvider serviceProvider = null;
        private CodeDomProvider codeDomProvider = null;

        public void SetSite(object pUnkSite)
        {
            site = pUnkSite;
            codeDomProvider = null;
            serviceProvider = null;
        }

        public void GetSite(ref Guid riid, out IntPtr ppvSite)
        {
            if (site == null)
            {
                throw new COMException("object is not sited", VSConstants.E_FAIL);
            }
            IntPtr pUnknownPointer = Marshal.GetIUnknownForObject(site);
            IntPtr intPointer;
            Marshal.QueryInterface(pUnknownPointer, ref riid, out intPointer);
            if (intPointer == IntPtr.Zero)
            {
                throw new COMException("site does not support requested interface", VSConstants.E_NOINTERFACE);
            }
            ppvSite = intPointer;
        }

        #endregion
    }
}
