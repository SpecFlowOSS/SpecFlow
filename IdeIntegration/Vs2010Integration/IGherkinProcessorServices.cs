using EnvDTE;
using gherkin;
using Microsoft.VisualStudio.Text;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Vs2010Integration.GherkinFileEditor;
using TechTalk.SpecFlow.Vs2010Integration.Options;

namespace TechTalk.SpecFlow.Vs2010Integration
{
    internal interface IGherkinProcessorServices
    {
        IntegrationOptions GetOptions();
        Project GetProject(ITextBuffer textBuffer);
        SpecFlowProject GetSpecFlowProjectFromProject(Project project);
        SpecFlowProject GetSpecFlowProjectFromFile(ITextBuffer textBuffer);
        I18n GetLanguageService(ITextBuffer textBuffer);
        GherkinFileEditorInfo GetParsingResult(ITextBuffer textBuffer);
        GherkinFileEditorParser GetParser(ITextBuffer textBuffer);
    }
}