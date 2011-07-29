using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ParserTests;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using Is=Rhino.Mocks.Constraints.Is;

namespace TechTalk.SpecFlow.RuntimeTests
{
    [TestFixture]
    public class ValidateStepAndEventOrdersTest : ExecutionTestBase
    {
        [Test, TestCaseSource(typeof(TestFileHelper), "GetTestFiles")]
        public void CanValidateStepAndEventOrdersForFile(string fileName)
        {
            ExecuteForFile(fileName);
        }

        protected override void ExecuteTests(object test, Feature feature)
        {
            MockRepository mockRepository = SetupTests(feature);

            NUnitTestExecutor.ExecuteNUnitTests(test, ex => true);

            mockRepository.VerifyAll();
        }

        private MockRepository SetupTests(Feature feature)
        {
            MockRepository mockRepository = new MockRepository();

            SetupTestRunnerMock(mockRepository, feature);

            mockRepository.ReplayAll();
            return mockRepository;
        }

        private void SetupTestRunnerMock(MockRepository mockRepository, Feature feature)
        {
            var testRunnerMock = mockRepository.StrictMock<ITestRunner>();
            ObjectContainer.SyncTestRunner = testRunnerMock;
            using (mockRepository.Ordered())
            {
                testRunnerMock.Expect(tr => tr.OnFeatureStart(null)).IgnoreArguments();

                using (mockRepository.Unordered())
                {
                    foreach (var scenario in feature.Scenarios)
                    {
                        if (scenario is ScenarioOutline)
                        {
                            ScenarioOutline scenarioOutline = (ScenarioOutline) scenario;
                            foreach (var exampleSet in scenarioOutline.Examples.ExampleSets)
                            {
                                foreach (var example in exampleSet.Table.Body)
                                {
                                    Dictionary<string, string> paramSubst = new Dictionary<string, string>();
                                    for (int i = 0; i < exampleSet.Table.Header.Cells.Length; i++)
                                    {
                                        paramSubst.Add(exampleSet.Table.Header.Cells[i].Value, example.Cells[i].Value);
                                    }

                                    SetupScenario(testRunnerMock, feature, scenario, paramSubst);
                                }
                            }
                        }
                        else
                        {
                            SetupScenario(testRunnerMock, feature, scenario, null);
                        }
                    }
                }

                testRunnerMock.Expect(tr => tr.OnFeatureEnd()).IgnoreArguments();
            }
        }

        private void SetupScenario(ITestRunner testRunnerMock, Feature feature, Scenario scenario, Dictionary<string, string> paramSubst)
        {
            testRunnerMock.Expect(tr => tr.OnScenarioStart(null)).IgnoreArguments();

            if (feature.Background != null)
                foreach (var step in feature.Background.Steps)
                    SetupStepMock(testRunnerMock, step, paramSubst);

            foreach (var step in scenario.Steps)
                SetupStepMock(testRunnerMock, step, paramSubst);

            testRunnerMock.Expect(tr => tr.CollectScenarioErrors()).IgnoreArguments();
            testRunnerMock.Expect(tr => tr.OnScenarioEnd()).IgnoreArguments();
        }

        private bool IsOf<TPhase>(ScenarioStep step)
        {
            return step is TPhase;
        }

        private void SetupStepMock(ITestRunner testRunnerMock, ScenarioStep step, Dictionary<string, string> paramSubst)
        {
            if (IsOf<Given>(step))
                AddStepConstraints(testRunnerMock.Expect(tr => tr.Given(null, null, null)), step, paramSubst);
            else if (IsOf<When>(step))
                AddStepConstraints(testRunnerMock.Expect(tr => tr.When(null, null, null)), step, paramSubst);
            else if (IsOf<Then>(step))
                AddStepConstraints(testRunnerMock.Expect(tr => tr.Then(null, null, null)), step, paramSubst);
            else if (IsOf<And>(step))
                AddStepConstraints(testRunnerMock.Expect(tr => tr.And(null, null, null)), step, paramSubst);
            else if (IsOf<But>(step))
                AddStepConstraints(testRunnerMock.Expect(tr => tr.But(null, null, null)), step, paramSubst);
        }

        private void AddStepConstraints(IMethodOptions<RhinoMocksExtensions.VoidType> methodOptions, ScenarioStep step, Dictionary<string, string> paramSubst)
        {
            methodOptions
                .IgnoreArguments()
                .Constraints(
                Is.Equal(GetReplacedText(step.Text, paramSubst)),
                Is.Equal(GetReplacedText(step.MultiLineTextArgument, paramSubst)),
                step.TableArg == null ? Is.Equal(null) : Is.NotNull());
        }

        private string GetReplacedText(string text, Dictionary<string, string> paramSubst)
        {
            if (text == null || paramSubst == null)
                return text;

            foreach (var subst in paramSubst)
            {
                text = text.Replace("<" + subst.Key + ">", subst.Value);
            }

            return text;
        }
    }
}