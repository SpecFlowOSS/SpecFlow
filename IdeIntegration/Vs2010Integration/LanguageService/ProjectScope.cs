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
        GherkinDialectServices GherkinDialectServices { get; }
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

        public GherkinDialectServices GherkinDialectServices
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
        private readonly GherkinTextBufferParser parser;

        public VsProjectScope(Project project, GherkinFileEditorClassifications classifications)
        {
            Classifications = classifications;
            this.project = project;
            //TODO: register for file changes, etc.

            parser = new GherkinTextBufferParser(this);
        }

        public GherkinTextBufferParser GherkinTextBufferParser
        {
            get { return parser; }
        }

        public GherkinScopeAnalyzer GherkinScopeAnalyzer
        {
            get { throw new NotImplementedException(); }
        }

        public GherkinDialectServices GherkinDialectServices
        {
            get { return new GherkinDialectServices(CultureInfo.GetCultureInfo("en-US")); }
        }

        public GherkinFileEditorClassifications Classifications { get; set; }
    }
}