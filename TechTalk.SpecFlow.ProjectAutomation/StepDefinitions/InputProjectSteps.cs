using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow.ProjectAutomation.StepDefinitions
{
    [Binding]
    public class InputProjectSteps
    {
        private readonly InputProjectDriver inputProjectDriver;

        public InputProjectSteps(InputProjectDriver inputProjectDriver)
        {
            this.inputProjectDriver = inputProjectDriver;
        }

        [Given(@"there is a custom report template file '(.*)' as")]
        public void GivenThereIsACustomReportTemplateFileAs(string fileName, string fileContent)
        {
            inputProjectDriver.AddContentFile(fileName, fileContent);
        }


        [Given(@"I have a feature file with (\d+) passing (\d+) failing and (\d+) pending scenarios")]
        public void GivenIHaveAFeatureFileWithPassingFailingAndPendingScenarios(int passCount, int failCount, int pendingCount)
        {
            StringBuilder featureBuilder = new StringBuilder();
            featureBuilder.AppendLine("Feature: Feature with different scenarios");

            foreach (var scenario in Enumerable.Range(0, passCount).Select(
                i => string.Format("Scenario: passing scenario nr {0}\r\nWhen the step pass", i)))
            {
                featureBuilder.AppendLine(scenario);
                featureBuilder.AppendLine();
            }

            foreach (var scenario in Enumerable.Range(0, failCount).Select(
                i => string.Format("Scenario: failing scenario nr {0}\r\nWhen the step fail", i)))
            {
                featureBuilder.AppendLine(scenario);
                featureBuilder.AppendLine();
            }

            foreach (var scenario in Enumerable.Range(0, pendingCount).Select(
                i => string.Format("Scenario: pending scenario nr {0}\r\nWhen the step is pending", i)))
            {
                featureBuilder.AppendLine(scenario);
                featureBuilder.AppendLine();
            }

            inputProjectDriver.AddFeatureFile(featureBuilder.ToString());

            inputProjectDriver.AddStepBinding(ScenarioBlock.When, "the step pass", code: "System.Threading.Thread.Sleep(new Random().Next(100));//pass");
            inputProjectDriver.AddStepBinding(ScenarioBlock.When, "the step fail", code: "System.Threading.Thread.Sleep(new Random().Next(100));throw new System.Exception(\"simulated failure\");");
        }

        [Given(@"there is a feature file with (\d+) scenarios displaying the app setting '(.*)' tagged with @(.*)")]
        public void GivenThereIsAFeatureFileWithNScenariosDisplayingTheAppSettingFooTagged(int count, string appSetting, string tag)
        {
            StringBuilder featureBuilder = new StringBuilder();
            if (tag != null)
                featureBuilder.AppendLine("@" + tag);
            featureBuilder.AppendLine("Feature: Feature with scenarios displaying app setting " + appSetting);

            foreach (var scenario in Enumerable.Range(0, count).Select(
                i => string.Format("Scenario: scenario nr {0}\r\nWhen the app setting {1} is displayed", i, appSetting)))
            {
                featureBuilder.AppendLine(scenario);
                featureBuilder.AppendLine();
            }

            inputProjectDriver.AddFeatureFile(featureBuilder.ToString());

            inputProjectDriver.AddStepBinding(ScenarioBlock.When, string.Format("the app setting {0} is displayed", appSetting),
                code: string.Format(@"System.Threading.Thread.Sleep(new Random().Next(100));
Console.WriteLine(""ExecutionFolder:"" + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
Console.WriteLine(""Config:"" + System.Configuration.ConfigurationManager.AppSettings[""{0}""]);", appSetting));
        }

        [Given(@"there is a feature file with (\d+) scenarios displaying the app setting '(.*)'")]
        public void GivenThereIsAFeatureFileWithNScenariosDisplayingTheAppSettingFoo(int count, string appSetting)
        {
            GivenThereIsAFeatureFileWithNScenariosDisplayingTheAppSettingFooTagged(count, appSetting, null);
        }


    }
}
