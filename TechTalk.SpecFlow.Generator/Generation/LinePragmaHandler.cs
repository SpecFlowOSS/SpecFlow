using System.CodeDom;
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

        public void AddLineDirectiveHidden(CodeStatementCollection statements)
        {
            if (_specFlowConfiguration.AllowDebugGeneratedFiles)
                return;

            _codeDomHelper.AddDisableSourceLinePragmaStatement(statements);
        }

        public void AddLineDirective(CodeStatementCollection statements, Background background)
        {
            AddLineDirective(statements, background.Location);
        }

        public void AddLineDirective(CodeStatementCollection statements, StepsContainer scenarioDefinition)
        {
            AddLineDirective(statements, scenarioDefinition.Location);
        }

        public void AddLineDirective(CodeStatementCollection statements, Step step)
        {
            AddLineDirective(statements, step.Location);
        }

        public void AddLineDirective(CodeStatementCollection statements, Location location)
        {
            if (location == null || _specFlowConfiguration.AllowDebugGeneratedFiles)
                return;

            _codeDomHelper.AddSourceLinePragmaStatement(statements, location.Line, location.Column);
        }
    }
}