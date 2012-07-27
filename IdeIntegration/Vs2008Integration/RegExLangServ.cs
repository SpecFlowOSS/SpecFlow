using System;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.Vs2008Integration
{
    /// <summary>
    /// This class implements language service that supplies syntax highlighting based on regular expression
    /// association table.
    /// </summary>

    // This attribute indicates that this managed type is visible to COM
    [ComVisible(true)]
    [Guid("0A485828-6D97-11E0-AFAC-304A4824019B")]
    class RegularExpressionLanguageService2 : LanguageService
    {
        private RegularExpressionScanner scanner;
        private LanguagePreferences preferences;
        private readonly DTE dte;

        public RegularExpressionLanguageService2(DTE dte)
        {
            this.dte = dte;
        }

        /// <summary>
        /// This method parses the source code based on the specified ParseRequest object.
        /// We don't need implement any logic here.
        /// </summary>
        /// <param name="req">The <see cref="ParseRequest"/> describes how to parse the source file.</param>
        /// <returns>If successful, returns an <see cref="AuthoringScope"/> object; otherwise, returns a null value.</returns>
        public override AuthoringScope ParseSource(ParseRequest req)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Language name property.
        /// </summary>        
        public override string Name
        {
            get { return "Regular Expression Language Service"; }
        }

        /// <summary>
        /// Returns a string with the list of the supported file extensions for this language service.
        /// </summary>
        /// <returns>Returns a LanguagePreferences object</returns>
        public override string GetFormatFilterList()
        {
            return VSPackage.RegExFormatFilter;
        }

        private Project CurrentProject
        {
            get
            {
                if (dte != null)
                {
                    return ((Project)((object[])dte.ActiveSolutionProjects)[0]);
                }
                throw new InvalidOperationException("Unable to detect current project.");
            }
        }

        /// <summary>
        /// Create and return instantiation of a parser represented by RegularExpressionScanner object.
        /// </summary>
        /// <param name="buffer">An <see cref="IVsTextLines"/> represents lines of source to parse.</param>
        /// <returns>Returns a RegularExpressionScanner object</returns>
        public override IScanner GetScanner(IVsTextLines buffer)
        {
            var configurationReader = new Vs2008SpecFlowConfigurationReader(CurrentProject, NullIdeTracer.Instance);
            var configurationHolder = configurationReader.ReadConfiguration();
            var config = new GeneratorConfigurationProvider().LoadConfiguration(configurationHolder) ??
                         new SpecFlowProjectConfiguration();

            scanner = new RegularExpressionScanner(config.GeneratorConfiguration.FeatureLanguage);

            return scanner;
        }

        /// <summary>
        /// Returns a <see cref="LanguagePreferences"/> object for this language service.
        /// </summary>
        /// <returns>Returns a LanguagePreferences object</returns>
        public override LanguagePreferences GetLanguagePreferences()
        {
            if (preferences == null)
            {
                // Create new LanguagePreferences instance
                preferences = new LanguagePreferences(this.Site, typeof(RegularExpressionLanguageService2).GUID, "Regular Expression Language Service");
            }

            return preferences;
        }
    }
}
