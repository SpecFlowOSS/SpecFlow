using System;
using System.CodeDom;
using System.Collections.Generic;
using Gherkin.Ast;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.Generator.CodeDom
{
    public class SourceLineScope : IDisposable
    {
        private readonly SpecFlowConfiguration _specFlowConfiguration;
        private readonly CodeDomHelper _codeDomHelper;
        private readonly List<CodeStatement> _statements;
        private readonly Location _location;

        public SourceLineScope(SpecFlowConfiguration specFlowConfiguration, CodeDomHelper codeDomHelper, List<CodeStatement> statements, string filename, Location location)
        {
            _specFlowConfiguration = specFlowConfiguration;
            _codeDomHelper = codeDomHelper;
            _statements = statements;
            _location = location;

            if (_location == null || _specFlowConfiguration.AllowDebugGeneratedFiles)
            {
                return;
            }
                
            _statements.AddRange(_codeDomHelper.CreateSourceLinePragmaStatement(filename, _location.Line, _location.Column));
        }

        public void Dispose()
        {
            if (_location == null || _specFlowConfiguration.AllowDebugGeneratedFiles)
            {
                return;
            }

            _statements.Add(_codeDomHelper.CreateDisableSourceLinePragmaStatement());
        }
    }
}