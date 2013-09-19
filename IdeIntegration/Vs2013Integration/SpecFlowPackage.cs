using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using BoDi;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using TechTalk.SpecFlow.IdeIntegration.Install;
using TechTalk.SpecFlow.Vs2010Integration.Commands;
using TechTalk.SpecFlow.Vs2010Integration.Options;

namespace TechTalk.SpecFlow.Vs2010Integration
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.9.1", IconResourceID = 400)]
    [ProvideOptionPageAttribute(typeof(OptionsPageGeneral), IntegrationOptionsProvider.SPECFLOW_OPTIONS_CATEGORY, IntegrationOptionsProvider.SPECFLOW_GENERAL_OPTIONS_PAGE, 121, 122, true)]
    [ProvideProfileAttribute(typeof(OptionsPageGeneral), IntegrationOptionsProvider.SPECFLOW_OPTIONS_CATEGORY, IntegrationOptionsProvider.SPECFLOW_GENERAL_OPTIONS_PAGE, 121, 123, true, DescriptionResourceID = 121)]
    [Guid(GuidList.guidSpecFlowPkgString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
    public sealed class SpecFlowPackagePackage : Package
    {
        public IObjectContainer Container { get; private set; }

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public SpecFlowPackagePackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }

        private IdeIntegration.Install.IdeIntegration? CurrentIdeIntegration
        {
            get
            {
                Version vsVersion;
                if (!Version.TryParse(Container.Resolve<EnvDTE.DTE>().Version, out vsVersion))
                    return null;

                switch (vsVersion.Major)
                {
                    case 10:
                        return IdeIntegration.Install.IdeIntegration.VisualStudio2010;
                    case 11:
                        return IdeIntegration.Install.IdeIntegration.VisualStudio2012;
                    default:
                        return null;
                }
            }
        }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            Container = VsContainerBuilder.CreateContainer(this);

            var currentIdeIntegration = CurrentIdeIntegration;
            if (currentIdeIntegration != null)
            {
                InstallServices installServices = Container.Resolve<InstallServices>();
                installServices.OnPackageLoad(currentIdeIntegration.Value);
            }

            OleMenuCommandService menuCommandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (menuCommandService != null)
            {
                foreach (var menuCommandHandler in Container.Resolve<IDictionary<SpecFlowCmdSet, MenuCommandHandler>>())
                {
                    menuCommandHandler.Value.RegisterTo(menuCommandService, menuCommandHandler.Key);
                }
            }
        }
    }
}
