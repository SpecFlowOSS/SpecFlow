using System;
using System.Linq;
using System.Windows;
using BoDi;
using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Install;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.TestRunner
{
    public class AutoTestRunnerGateway : ITestRunnerGateway
    {
        private readonly IObjectContainer container;
        private readonly IdeIntegration.Install.IdeIntegration ideIntegration;

        public AutoTestRunnerGateway(IObjectContainer container, InstallServices installServices)
        {
            this.container = container;
            ideIntegration = installServices.IdeIntegration;
        }

        private ITestRunnerGateway GetCurrentTestRunnerGateway(Project project)
        {
            if (VsxHelper.GetReference(project, "TechTalk.SpecRun") != null)
                return container.Resolve<ITestRunnerGateway>(TestRunnerTool.SpecRun.ToString());

            var reSharperAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "JetBrains.ReSharper.UnitTestFramework");

            if (reSharperAssembly != null)
            {
                if (reSharperAssembly.GetName().Version.Major <= 5)
                    return container.Resolve<ITestRunnerGateway>(TestRunnerTool.ReSharper5.ToString());

                return container.Resolve<ITestRunnerGateway>(TestRunnerTool.ReSharper.ToString());
            }

            if (ideIntegration == IdeIntegration.Install.IdeIntegration.VisualStudio2012)
            {
                return container.Resolve<ITestRunnerGateway>(TestRunnerTool.VisualStudio2012.ToString());
            }

            if (VsxHelper.GetReference(project, "Microsoft.VisualStudio.QualityTools.UnitTestFramework") != null)
                return container.Resolve<ITestRunnerGateway>(TestRunnerTool.VisualStudio2010MsTest.ToString());

            MessageBox.Show(
                "Could not find matching test runner. Please specify the test runner tool in 'Tools / Options / SpecFlow'",
                "SpecFlow", MessageBoxButton.OK, MessageBoxImage.Error);

            return null;
        }

        public bool RunScenario(ProjectItem projectItem, IScenarioBlock currentScenario, IGherkinFileScope fileScope, bool debug)
        {
            var testRunnerGateway = GetCurrentTestRunnerGateway(projectItem.ContainingProject);
            if (testRunnerGateway == null)
                return false;

            return testRunnerGateway.RunScenario(projectItem, currentScenario, fileScope, debug);
        }

        public bool RunFeatures(ProjectItem projectItem, bool debug)
        {
            var testRunnerGateway = GetCurrentTestRunnerGateway(projectItem.ContainingProject);
            if (testRunnerGateway == null)
                return false;

            return testRunnerGateway.RunFeatures(projectItem, debug);
        }

        public bool RunFeatures(Project project, bool debug)
        {
            var testRunnerGateway = GetCurrentTestRunnerGateway(project);
            if (testRunnerGateway == null)
                return false;

            return testRunnerGateway.RunFeatures(project, debug);
        }
    }
}