using System;
using System.Collections.Generic;
using System.Globalization;
using EnvDTE;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public interface IProjectScope
    {
        GherkinTextBufferParser GherkinTextBufferParser { get; }
        GherkinScopeAnalyzer GherkinScopeAnalyzer { get; }
        GherkinLanguageServices GherkinLanguageServices { get; }
        GherkinFileEditorClassifications Classifications { get; }
    }

    internal class NoProjectScope : IProjectScope
    {
        public static NoProjectScope Instance = new NoProjectScope();

        #region Implementation of IProjectScope

        public GherkinTextBufferParser GherkinTextBufferParser
        {
            get { throw new NotImplementedException(); }
        }

        public GherkinScopeAnalyzer GherkinScopeAnalyzer
        {
            get { throw new NotImplementedException(); }
        }

        public GherkinLanguageServices GherkinLanguageServices
        {
            get { throw new NotImplementedException(); }
        }

        public GherkinFileEditorClassifications Classifications
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }

    public class VsProjectScope : IProjectScope
    {
        private Project project;
        private GherkinTextBufferParser parser;

        public VsProjectScope(Project project)
        {
            this.project = project;
            //TODO: register for file changes, etc.

            parser = new GherkinTextBufferParser(this);
        }

        #region Implementation of IProjectScope

        public GherkinTextBufferParser GherkinTextBufferParser
        {
            get { return parser; }
        }

        public GherkinScopeAnalyzer GherkinScopeAnalyzer
        {
            get { throw new NotImplementedException(); }
        }

        public GherkinLanguageServices GherkinLanguageServices
        {
            get { return new GherkinLanguageServices(CultureInfo.GetCultureInfo("en-US")); }
        }

        public GherkinFileEditorClassifications Classifications
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}