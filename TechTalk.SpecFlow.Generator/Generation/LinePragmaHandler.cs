using System.CodeDom;
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

            _codeDomHelper.BindTypeToSourceFile(testType, sourceFile);
        }
    }
}