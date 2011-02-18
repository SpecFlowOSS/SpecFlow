using EnvDTE;
using Microsoft.VisualStudio.Text;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Vs2010Integration.Options;

namespace TechTalk.SpecFlow.Vs2010Integration
{
    internal interface ISpecFlowServices
    {
        IntegrationOptions GetOptions();
        Project GetProject(ITextBuffer textBuffer);
    }
}