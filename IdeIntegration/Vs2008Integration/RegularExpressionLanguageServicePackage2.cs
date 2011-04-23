//***************************************************************************
//
//    Copyright (c) Microsoft Corporation. All rights reserved.
//    This code is licensed under the Visual Studio SDK license terms.
//    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
//    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
//    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
//    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//***************************************************************************

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Project;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.VsIntegration.Common;
using VSOLE = Microsoft.VisualStudio.OLE.Interop;


namespace TechTalk.SpecFlow.Vs2008Integration
{
    /// <summary>
    /// This class implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    
    // A Visual Studio component can be registered under different registry roots; for instance
    // when you debug your package you want to register it in the experimental hive. This
    // attribute specifies the registry root to use if no one is provided to regpkg.exe with
    // the /root switch.
    [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0Exp")]
    
    // This attribute tells the registration utility (regpkg.exe) that this class needs
    // to be registered as package.
    [PackageRegistration(UseManagedResourcesOnly = true)]

    // This attribute is used to register the information needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration(false, "#100", "#102", "1.0", IconResourceID = 400)]

    // In order to be loaded inside Visual Studio in a machine that has not the VS SDK installed, 
    // package needs to have a valid load key (it can be requested at 
    // http://msdn.microsoft.com/vstudio/extend/). This attributes tells the shell that this 
    // package has a load key embedded in its resources.
    [ProvideLoadKey("Standard", "1.0", "Regular Expression Language Service", "Microsoft Corporation", 1)]

    // This attribute is used to associate the ".feature" file extension with a language service
    [ProvideLanguageExtension(typeof(RegularExpressionLanguageService2), ".feature")]

    // This attribute is needed to indicate that the type proffers a service
    [ProvideService(typeof(RegularExpressionLanguageService2))]

    // Indicates that this managed type is visible to COM
    [ComVisible(true)]

    [Guid("C0328222-6D96-11E0-9525-E7494824019B")]
    public sealed class RegularExpressionLanguageServicePackage2 : Package, IDisposable
    {
        private RegularExpressionLanguageService2 _langService2;
        
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public RegularExpressionLanguageServicePackage2()
        {

            var specFlowProjectConfiguration = LoadConfiguration();
            
            //var gherkinDialectServices = new GherkinDialectServices(specFlowProjectConfiguration.GeneratorConfiguration.FeatureLanguage);
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }

        protected override void OnLoadOptions(string key, System.IO.Stream stream)
        {
            base.OnLoadOptions(key, stream);
        }

        private SpecFlowProjectConfiguration LoadConfiguration()
        {

            return null;
        }
        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited.
        /// Creation of instances which realize Regular Expression Language services and putting 
        /// those instances to package services container.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // Create instance of RegularExpressionLanguageService2 type
            _langService2 = new RegularExpressionLanguageService2();
            _langService2.SetSite(this);

            // Add our language service object to VSPackage services container
            IServiceContainer sc = (IServiceContainer)this;
            sc.AddService(typeof(RegularExpressionLanguageService2), _langService2, true);

            DTE dte = (DTE)this.GetService(typeof(SDTE));
            
        }

        /// <summary>
        /// This method is used to dispose all object's resources
        /// </summary>
        /// <param name="disposing">If disposing is true, the method has been called directly
        /// or indirectly by a user's code.
        /// If disposing is false, the method has been called by the runtime.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    // Dispose managed resources
                    if (_langService2 != null)
                    {
                        _langService2.Dispose();
                    }
                }
            }
            finally
            {
                // Call base class handler
                base.Dispose(disposing);
            }
        }

        #endregion       
    
        #region IDisposable Members
        
        /// <summary>
        /// Implements IDisposable.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}