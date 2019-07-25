using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using Gherkin.Ast;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.CodeDom;

namespace TechTalk.SpecFlow.Generator.Generation
{
    public class LinePragmaHandler
    {
        private readonly SpecFlowConfiguration _specFlowConfiguration;
        private readonly CodeDomHelper _codeDomHelper;

        public LinePragmaHandler(SpecFlowConfiguration specFlowConfiguration, CodeDomHelper codeDomHelper)
        {
            _specFlowConfiguration = specFlowConfiguration;
            _codeDomHelper = codeDomHelper;
        }


        public void AddLinePragmaInitial(CodeTypeDeclaration testType, string sourceFile)
        {
            if (_specFlowConfiguration.AllowDebugGeneratedFiles)
                return;

            _codeDomHelper.BindTypeToSourceFile(testType, Path.GetFileName(sourceFile));
        }




        public IEnumerable<CodeStatement> CreateLineDirective(string filename, Location location)
        {
            if (location == null || _specFlowConfiguration.AllowDebugGeneratedFiles)
                return new List<CodeStatement>();

            return _codeDomHelper.CreateSourceLinePragmaStatement(filename, location.Line, location.Column);
        }
    }
}