using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Designer.Interfaces;
using Microsoft.VisualStudio.Shell.Interop;
using TechTalk.SpecFlow.Utils;
using VSOLE = Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;

namespace TechTalk.SpecFlow.VsIntegration.Common
{
    [System.Runtime.InteropServices.ComVisible(true)]
    [Guid("800FD294-E1AF-4a80-AFF2-FFBCE664D020")]
    public abstract partial class BaseCodeGeneratorWithSite : BaseCodeGenerator, VSOLE.IObjectWithSite
    {
        private object site = null;
        private ServiceProvider serviceProvider = null;
        private CodeDomProvider codeDomProvider = null;

        void VSOLE.IObjectWithSite.SetSite(object pUnkSite)
        {
            site = pUnkSite;
            codeDomProvider = null;
            serviceProvider = null;
        }

        void VSOLE.IObjectWithSite.GetSite(ref Guid riid, out IntPtr ppvSite)
        {
            if (site == null)
            {
                throw new COMException("object is not sited",
                                       VSConstants.E_FAIL);
            }
            IntPtr pUnknownPointer = Marshal.GetIUnknownForObject(site);
            IntPtr intPointer = IntPtr.Zero;
            Marshal.QueryInterface(pUnknownPointer, ref riid, out intPointer);
            if (intPointer == IntPtr.Zero)
            {
                throw new COMException(
                    "site does not support requested interface",
                    VSConstants.E_NOINTERFACE);
            }
            ppvSite = intPointer;
        }

        private ServiceProvider SiteServiceProvider
        {
            get
            {
                if (serviceProvider == null)
                {
                    serviceProvider =
                      new ServiceProvider(site as VSOLE.IServiceProvider);
                }
                return serviceProvider;
            }
        }

        protected object GetService(Type serviceType)
        {
            return SiteServiceProvider.GetService(serviceType);
        }

        protected virtual CodeDomProvider GetCodeProvider()
        {
            if (codeDomProvider == null)
            {
                IVSMDCodeDomProvider provider =
                  GetService(typeof(SVSMDCodeDomProvider))
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

        protected override string GetGeneratedCodeForFailure(string errorMessage)
        {
            CodeDomHelper codeDomHelper = new CodeDomHelper(GetCodeProvider());

            string[] messageLines = errorMessage.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);

            var codeLines = messageLines.Select(codeDomHelper.GetErrorStatementString).ToArray();

            return string.Join(Environment.NewLine, codeLines);
        }

        public static bool Failed(int hr)
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
                    VSOLE.IServiceProvider ppSP = null;
                    if (!Failed(service.GetSite(out ppSP)) && (ppSP != null))
                    {
                        Guid gUID = typeof(DTE).GUID;
                        IntPtr zero = IntPtr.Zero;
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

        protected IVsHierarchy VsHierarchy
        {
            get
            {
                return this.GetService(typeof (IVsHierarchy)) as IVsHierarchy;
            }
        }

        protected IVsErrorList ErrorList
        {
            get
            {
                IVsErrorList objectForIUnknown = null;
                IVsHierarchy service = this.GetService(typeof(IVsHierarchy)) as IVsHierarchy;
                if (service != null)
                {
                    VSOLE.IServiceProvider ppSP = null;
                    if (!Failed(service.GetSite(out ppSP)) && (ppSP != null))
                    {
                        Guid gUID = typeof(SVsErrorList).GUID;
                        Guid riid = typeof(IVsErrorList).GUID;
                        IntPtr zero = IntPtr.Zero;
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

        public Project CurrentProject
        {
            get
            {
                DTE dte = Dte;
                return GetProjectForSourceFile(dte);
            }
        }

        private Project GetProjectForSourceFile(DTE dte)
        {
            if (dte != null)
            {
                ProjectItem prjItem = dte.Solution.FindProjectItem(CodeFilePath);
                if (prjItem != null)
                    return prjItem.ContainingProject;
            }
            throw new InvalidOperationException("Unable to detect current project.");
        }

        /*
        private void SetTargetNamespace()
        {
            if (!string.IsNullOrEmpty(this.CodeFileNameSpace))
            {
                TargetNamespace = CodeFileNameSpace;
                return;
            }

            DTE dte = Dte;
            if (dte == null)
                return;
            IEnumerable activeProjects = dte.ActiveSolutionProjects as IEnumerable;
            if (activeProjects == null)
                return;
            Project project = activeProjects.OfType<Project>().First();
            if (project == null)
                return;
            var defaultNamespace = project.Properties.Item("DefaultNamespace");
            if (defaultNamespace != null && defaultNamespace.Value is string)
            {
                string targetNamespace = (string)defaultNamespace.Value;
                string projectFolder = Path.GetDirectoryName(project.FullName);
                string sourceFileFolder = Path.GetDirectoryName(this.CodeFilePath);
                if (sourceFileFolder.StartsWith(sourceFileFolder, StringComparison.InvariantCultureIgnoreCase))
                {
                    string extraFolders = sourceFileFolder.Substring(projectFolder.Length);
                    if (extraFolders.Length > 0)
                    {
                        string[] parts = extraFolders.TrimStart('\\').Split('\\');
                        targetNamespace += "." + string.Join(".",
                            parts.Select(p => p.ToIdentifier()).ToArray());
                    }
                    targetNamespace += extraFolders.Replace("\\", ".");
                }
                TargetNamespace = targetNamespace;
            }
        }
*/

        protected override void AfterCodeGenerated(bool error)
        {
            base.AfterCodeGenerated(error);

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
    }
}