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
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace TechTalk.SpecFlow.Vs2008Integration
{
    /// <summary>
    /// This class implements language service that supplies syntax highlighting based on regular expression
    /// association table.
    /// </summary>
    
    // This attribute indicates that this managed type is visible to COM
    [ComVisible(true)]
    //[Guid("C674518A-3127-4f00-9C4D-BE0EAAB8C761")]
    [Guid("0A485828-6D97-11E0-AFAC-304A4824019B")]
    class RegularExpressionLanguageService2 : LanguageService
    {
        private RegularExpressionScanner scanner;

        private LanguagePreferences preferences;

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

        /// <summary>
        /// Create and return instantiation of a parser represented by RegularExpressionScanner object.
        /// </summary>
        /// <param name="buffer">An <see cref="IVsTextLines"/> represents lines of source to parse.</param>
        /// <returns>Returns a RegularExpressionScanner object</returns>
        public override IScanner GetScanner(IVsTextLines buffer)
        {
            if (scanner == null)
            {
                // Create new RegularExpressionScanner instance
                scanner = new RegularExpressionScanner();
            }

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
