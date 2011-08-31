using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using Microsoft.VisualStudio.Text;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;

namespace TechTalk.SpecFlow.Vs2010Integration.TestRunner
{
    public interface ITestRunnerGateway
    {
        bool RunScenario(ProjectItem projectItem, IScenarioBlock currentScenario, IGherkinFileScope fileScope);
        bool RunFeatures(ProjectItem projectItem);
        bool RunFeatures(Project project);
    }
}
