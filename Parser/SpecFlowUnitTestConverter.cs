using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using TechTalk.SpecFlow.Parser.UnitTestProvider;

namespace TechTalk.SpecFlow.Parser
{
    public class SpecFlowUnitTestConverter
    {
        private const string DEFAULT_NAMESPACE = "SpecFlowTests";
        const string FIXTURE_FORMAT = "{0}Feature";
        const string TEST_FORMAT = "{0}";
        private const string IGNORE_TAG = "Ignore";
        private const string SETUP_NAME = "ScenarioSetup";
        private const string TEARDOWN_NAME = "ScenarioTearDown";
        private const string FIXTURESETUP_NAME = "FeatureSetup";
        private const string FIXTURETEARDOWN_NAME = "FeatureTearDown";
        private const string BACKGROUND_NAME = "FeatureBackground";
        private const string TESTRUNNER_FIELD = "testRunner";
        private const string ITESTRUNNER_TYPE = "TechTalk.SpecFlow.ITestRunner";
        private const string TESTRUNNERMANAGER_TYPE = "TechTalk.SpecFlow.TestRunnerManager";
        private const string FEATUREINFO_TYPE = "TechTalk.SpecFlow.FeatureInfo";
        private const string SCENARIOINFO_TYPE = "TechTalk.SpecFlow.ScenarioInfo";
        private const string TABLE_TYPE = "TechTalk.SpecFlow.Table";
        private const string SPECFLOW_NAMESPACE = "TechTalk.SpecFlow";

        private readonly IUnitTestGeneratorProvider testGeneratorProvider;

        public SpecFlowUnitTestConverter(IUnitTestGeneratorProvider testGeneratorProvider)
        {
            this.testGeneratorProvider = testGeneratorProvider;
        }

        public CodeCompileUnit GenerateUnitTestFixture(Feature feature, string testClassName, string targetNamespace)
        {
            targetNamespace = targetNamespace ?? DEFAULT_NAMESPACE;
            testClassName = testClassName ?? string.Format(FIXTURE_FORMAT, feature.Title.ToIdentifier());

            CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
            CodeNamespace codeNamespace = new CodeNamespace(targetNamespace);
            codeCompileUnit.Namespaces.Add(codeNamespace);

            codeNamespace.Imports.Add(new CodeNamespaceImport(SPECFLOW_NAMESPACE));

            var testType = new CodeTypeDeclaration(testClassName);
            testType.IsPartial = true;
            testType.TypeAttributes |= TypeAttributes.Public;
            codeNamespace.Types.Add(testType);

            testGeneratorProvider.SetTestFixture(testType, feature.Title, feature.Description);
            if (feature.Tags != null)
            {
                testGeneratorProvider.SetTestFixtureCategories(testType, GetNonIgnoreTags(feature.Tags));
                if (feature.Tags.Any(t => t.Name.Equals(IGNORE_TAG, StringComparison.InvariantCultureIgnoreCase)))
                    testGeneratorProvider.SetIgnore(testType);
            }

            DeclareTestRunnerMember(testType);

            GenerateTestFixtureSetup(testType, feature);
            GenerateTestFixtureTearDown(testType);
            var testSetup = GenerateTestSetup(testType);
            GenerateTestTearDown(testType);

            if (feature.Background != null)
                GenerateBackground(testType, testSetup, feature.Background);

            foreach (var scenario in feature.Scenarios)
            {
                if (scenario is ScenarioOutline)
                    GenerateScenarioOutlineTest(testType, testSetup, (ScenarioOutline)scenario, feature);
                else
                    GenerateTest(testType, testSetup, scenario, feature);
            }

            return codeCompileUnit;
        }

        private void DeclareTestRunnerMember(CodeTypeDeclaration testType)
        {
            CodeMemberField testRunnerField = new CodeMemberField(ITESTRUNNER_TYPE, TESTRUNNER_FIELD);
            testRunnerField.Attributes |= MemberAttributes.Static;
            testType.Members.Add(testRunnerField);
        }

        private CodeExpression GetTestRunnerExpression()
        {
            return new CodeVariableReferenceExpression(TESTRUNNER_FIELD);
            //return new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), TESTRUNNER_FIELD);
        }

        private IEnumerable<string> GetNonIgnoreTags(IEnumerable<Tag> tags)
        {
            return tags.Where(t => !t.Name.Equals(IGNORE_TAG, StringComparison.InvariantCultureIgnoreCase)).Select(t => t.Name);
        }

        private CodeMemberMethod GenerateTestFixtureSetup(CodeTypeDeclaration testType, Feature feature)
        {
            CodeMemberMethod setupMethod = new CodeMemberMethod();
            testType.Members.Add(setupMethod);

            setupMethod.Attributes = MemberAttributes.Public;
            setupMethod.Name = FIXTURESETUP_NAME;

            testGeneratorProvider.SetTestFixtureSetup(setupMethod);

            //testRunner = TestRunner.GetTestRunner();
            var testRunnerField = GetTestRunnerExpression();
            setupMethod.Statements.Add(
                new CodeAssignStatement(
                    testRunnerField,
                    new CodeMethodInvokeExpression(
                        new CodeTypeReferenceExpression(TESTRUNNERMANAGER_TYPE),
                        "GetTestRunner")));

            //FeatureInfo featureInfo = new FeatureInfo("xxxx");
            setupMethod.Statements.Add(
                new CodeVariableDeclarationStatement(FEATUREINFO_TYPE, "featureInfo",
                    new CodeObjectCreateExpression(FEATUREINFO_TYPE,
                        new CodeObjectCreateExpression(typeof(CultureInfo), 
                            new CodePrimitiveExpression(feature.Language)),
                        new CodePrimitiveExpression(feature.Title),
                        new CodePrimitiveExpression(feature.Description),
                        GetStringArrayExpression(feature.Tags))));

            //testRunner.OnFeatureStart(featureInfo);
            setupMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    testRunnerField,
                    "OnFeatureStart",
                    new CodeVariableReferenceExpression("featureInfo")));

            return setupMethod;
        }

        private CodeExpression GetStringArrayExpression(Tags tags)
        {
            if (tags == null || tags.Count == 0)
                return new CodeCastExpression(typeof(string[]), new CodePrimitiveExpression(null));

            List<CodeExpression> items = new List<CodeExpression>();
            foreach (var tag in tags)
            {
                items.Add(new CodePrimitiveExpression(tag.Name));
            }

            return new CodeArrayCreateExpression(typeof (string[]), items.ToArray());
        }

        private CodeExpression GetStringArrayExpression(IEnumerable<string> items, ParameterSubstitution paramToIdentifier)
        {
            List<CodeExpression> expressions = new List<CodeExpression>();
            foreach (var item in items)
            {
                expressions.Add(GetSubstitutedString(item, paramToIdentifier));
            }

            return new CodeArrayCreateExpression(typeof (string[]), expressions.ToArray());
        }

        private CodeMemberMethod GenerateTestFixtureTearDown(CodeTypeDeclaration testType)
        {
            CodeMemberMethod tearDownMethod = new CodeMemberMethod();
            testType.Members.Add(tearDownMethod);

            tearDownMethod.Attributes = MemberAttributes.Public;
            tearDownMethod.Name = FIXTURETEARDOWN_NAME;

            testGeneratorProvider.SetTestFixtureTearDown(tearDownMethod);

            var testRunnerField = GetTestRunnerExpression();
//            testRunner.OnFeatureEnd();
            tearDownMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    testRunnerField,
                    "OnFeatureEnd"));
//            testRunner = null;
            tearDownMethod.Statements.Add(
                new CodeAssignStatement(
                    testRunnerField,
                    new CodePrimitiveExpression(null)));

            return tearDownMethod;
        }

        private CodeMemberMethod GenerateTestTearDown(CodeTypeDeclaration testType)
        {
            CodeMemberMethod tearDownMethod = new CodeMemberMethod();
            testType.Members.Add(tearDownMethod);

            tearDownMethod.Attributes = MemberAttributes.Public;
            tearDownMethod.Name = TEARDOWN_NAME;

            testGeneratorProvider.SetTestTearDown(tearDownMethod);

            var testRunnerField = GetTestRunnerExpression();
            //testRunner.OnScenarioEnd();
            tearDownMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    testRunnerField,
                    "OnScenarioEnd"));

            return tearDownMethod;
        }

        private CodeMemberMethod GenerateTestSetup(CodeTypeDeclaration testType)
        {
            CodeMemberMethod setupMethod = new CodeMemberMethod();
            testType.Members.Add(setupMethod);

            setupMethod.Attributes = MemberAttributes.Public;
            setupMethod.Name = SETUP_NAME;
            setupMethod.Parameters.Add(
                new CodeParameterDeclarationExpression(SCENARIOINFO_TYPE, "scenarioInfo"));

            //testRunner.OnScenarioStart(scenarioInfo);
            var testRunnerField = GetTestRunnerExpression();
            setupMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    testRunnerField,
                    "OnScenarioStart",
                    new CodeVariableReferenceExpression("scenarioInfo")));

            return setupMethod;
        }

        private void GenerateBackground(CodeTypeDeclaration testType, CodeMemberMethod testSetup, Background background)
        {
            CodeMemberMethod backgroundMethod = new CodeMemberMethod();
            testType.Members.Add(backgroundMethod);

            backgroundMethod.Attributes = MemberAttributes.Public;
            backgroundMethod.Name = BACKGROUND_NAME;

            foreach (var given in background.Steps)
                GenerateStep(backgroundMethod, given, null);

            testSetup.Statements.Add(
                new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    backgroundMethod.Name));
        }

        private class ParameterSubstitution : List<KeyValuePair<string, string>>
        {
            public void Add(string parameter, string identifier)
            {
                base.Add(new KeyValuePair<string, string>(parameter.Trim(), identifier));
            }

            public bool TryGetIdentifier(string param, out string id)
            {
                param = param.Trim();
                foreach (var pair in this)
                {
                    if (pair.Key.Equals(param))
                    {
                        id = pair.Value;
                        return true;
                    }
                }
                id = null;
                return false;
            }
        }

        private void GenerateScenarioOutlineTest(CodeTypeDeclaration testType, CodeMemberMethod testSetup, ScenarioOutline scenarioOutline, Feature feature)
        {
            string testMethodName = string.Format(TEST_FORMAT, scenarioOutline.Title.ToIdentifier());

            ParameterSubstitution paramToIdentifier = new ParameterSubstitution();
            foreach (var param in scenarioOutline.Examples.ExampleSets[0].Table.Header.Cells)
                paramToIdentifier.Add(param.Value, param.Value.ToIdentifierCamelCase());

            if (scenarioOutline.Examples.ExampleSets.Length > 1)
            {
                //TODO: check params
            }

            GenerateScenarioOutlineBody(scenarioOutline, paramToIdentifier, testType, testMethodName, testSetup);

            int exampleSetIndex = 0;
            foreach (var exampleSet in scenarioOutline.Examples.ExampleSets)
            {
                string exampleSetTitle = exampleSet.Title == null ? string.Format("Scenarios{0}", exampleSetIndex + 1) :
                    exampleSet.Title.ToIdentifier();

                bool useFirstColumnAsName = CanUseFirstColumnAsName(exampleSet.Table);

                for (int rowIndex = 0; rowIndex < exampleSet.Table.Body.Length; rowIndex++)
                {
                    string variantName = useFirstColumnAsName ? exampleSet.Table.Body[rowIndex].Cells[0].Value.ToIdentifier() :
                        string.Format("Variant{0}", rowIndex);
                    GenerateScenarioOutlineTestVariant(testType, scenarioOutline, testMethodName, paramToIdentifier, exampleSetTitle, exampleSet.Table.Body[rowIndex], variantName);
                }
                exampleSetIndex++;
            }
        }

        private bool CanUseFirstColumnAsName(Table table)
        {
            if (table.Header.Cells.Length == 0)
                return false;

            return table.Body.Select(r => r.Cells[0].Value.ToIdentifier()).Distinct().Count() == table.Body.Length;
        }

        private void GenerateScenarioOutlineBody(ScenarioOutline scenarioOutline, ParameterSubstitution paramToIdentifier, CodeTypeDeclaration testType, string testMethodName, CodeMemberMethod testSetup)
        {
            CodeMemberMethod testMethod = new CodeMemberMethod();
            testType.Members.Add(testMethod);

            testMethod.Attributes = MemberAttributes.Public;
            testMethod.Name = testMethodName;

            foreach (var pair in paramToIdentifier)
            {
                testMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof (string), pair.Value));
            }

            GenerateTestBody(scenarioOutline, testMethod, testSetup, paramToIdentifier);
        }

        private void GenerateScenarioOutlineTestVariant(CodeTypeDeclaration testType, ScenarioOutline scenarioOutline, string testMethodName, List<KeyValuePair<string, string>> paramToIdentifier, string exampleSetTitle, Row row, string variantName)
        {
            CodeMemberMethod testMethod = GetTestMethodDeclaration(testType, scenarioOutline);
            testMethod.Name = string.IsNullOrEmpty(exampleSetTitle) ?
                string.Format("{0}_{1}", testMethod.Name, variantName) :
                string.Format("{0}_{1}_{2}", testMethod.Name, exampleSetTitle, variantName);

            //call test implementation with the params
            List<CodeExpression> argumentExpressions = new List<CodeExpression>();
            foreach (var paramCell in row.Cells)
            {
                argumentExpressions.Add(new CodePrimitiveExpression(paramCell.Value));
            }
            testMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    testMethodName,
                    argumentExpressions.ToArray()));
        }

        private void GenerateTest(CodeTypeDeclaration testType, CodeMemberMethod testSetup, Scenario scenario, Feature feature)
        {
            CodeMemberMethod testMethod = GetTestMethodDeclaration(testType, scenario);
            GenerateTestBody(scenario, testMethod, testSetup, null);
        }

        private void GenerateTestBody(Scenario scenario, CodeMemberMethod testMethod, CodeMemberMethod testSetup, ParameterSubstitution paramToIdentifier)
        {
            //call test setup
            //ScenarioInfo scenarioInfo = new ScenarioInfo("xxxx", tags...);
            testMethod.Statements.Add(
                new CodeVariableDeclarationStatement(SCENARIOINFO_TYPE, "scenarioInfo",
                    new CodeObjectCreateExpression(SCENARIOINFO_TYPE,
                        new CodePrimitiveExpression(scenario.Title),
                        GetStringArrayExpression(scenario.Tags))));

            testMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    testSetup.Name,
                    new CodeVariableReferenceExpression("scenarioInfo")));

            foreach (var scenarioStep in scenario.Steps)
            {
                GenerateStep(testMethod, scenarioStep, paramToIdentifier);
            }


            // call collect errors
            var testRunnerField = GetTestRunnerExpression();
            //testRunner.CollectScenarioErrors();
            testMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    testRunnerField,
                    "CollectScenarioErrors"));

        }

        private CodeMemberMethod GetTestMethodDeclaration(CodeTypeDeclaration testType, Scenario scenario)
        {
            CodeMemberMethod testMethod = new CodeMemberMethod();
            testType.Members.Add(testMethod);
            
            testMethod.Attributes = MemberAttributes.Public;
            testMethod.Name = string.Format(TEST_FORMAT, scenario.Title.ToIdentifier());

            testGeneratorProvider.SetTest(testMethod, scenario.Title);
            if (scenario.Tags != null)
            {
                testGeneratorProvider.SetTestCategories(testMethod, GetNonIgnoreTags(scenario.Tags));
                if (scenario.Tags.Any(t => t.Name.Equals(IGNORE_TAG, StringComparison.InvariantCultureIgnoreCase)))
                    testGeneratorProvider.SetIgnore(testMethod);
            }
            return testMethod;
        }

        private CodeExpression GetSubstitutedString(string text, ParameterSubstitution paramToIdentifier)
        {
            if (text == null)
                return new CodeCastExpression(typeof(string), new CodePrimitiveExpression(null));
            if (paramToIdentifier == null)
                return new CodePrimitiveExpression(text);

            Regex paramRe = new Regex(@"\<(?<param>[^\>]+)\>");
            string formatText = text.Replace("{", "{{").Replace("}", "}}");
            List<string> arguments = new List<string>();

            formatText = paramRe.Replace(formatText, match =>
                                             {
                                                 string param = match.Groups["param"].Value;
                                                 string id;
                                                 if (!paramToIdentifier.TryGetIdentifier(param, out id))
                                                     return match.Value;
                                                 int argIndex = arguments.IndexOf(id);
                                                 if (argIndex < 0)
                                                 {
                                                     argIndex = arguments.Count;
                                                     arguments.Add(id);
                                                 }
                                                 return "{" + argIndex + "}";
                                             });

            if (arguments.Count == 0)
                return new CodePrimitiveExpression(text);

            List<CodeExpression> formatArguments = new List<CodeExpression>();
            formatArguments.Add(new CodePrimitiveExpression(formatText));
            foreach (var id in arguments)
                formatArguments.Add(new CodeVariableReferenceExpression(id));

            return new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof (string)),
                "Format",
                formatArguments.ToArray());
        }

        private void GenerateStep(CodeMemberMethod testMethod, ScenarioStep scenarioStep, ParameterSubstitution paramToIdentifier)
        {
            var testRunnerField = GetTestRunnerExpression();

            //testRunner.Given("something");
            List<CodeExpression> arguments = new List<CodeExpression>();
            arguments.Add(
                GetSubstitutedString(scenarioStep.Text, paramToIdentifier));
            if (scenarioStep.MultiLineTextArgument != null || scenarioStep.TableArg != null)
            {
                arguments.Add(
                    GetMultilineTextArgExpression(scenarioStep.MultiLineTextArgument, paramToIdentifier));
                arguments.Add(
                    GetTableArgExpression(scenarioStep.TableArg, testMethod.Statements, paramToIdentifier));
            }

            testMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    testRunnerField,
                    scenarioStep.GetType().Name,
                    arguments.ToArray()));
        }

        private int tableCounter = 0;
        private CodeExpression GetTableArgExpression(Table tableArg, CodeStatementCollection statements, ParameterSubstitution paramToIdentifier)
        {
            if (tableArg == null)
                return new CodeCastExpression(TABLE_TYPE, new CodePrimitiveExpression(null));

            tableCounter++;

            //Table table0 = new Table(header...);
            var tableVar = new CodeVariableReferenceExpression("table" + tableCounter);
            statements.Add(
                new CodeVariableDeclarationStatement(TABLE_TYPE, tableVar.VariableName,
                        new CodeObjectCreateExpression(
                            TABLE_TYPE,
                            GetStringArrayExpression(tableArg.Header.Cells.Select(c => c.Value), paramToIdentifier))));

            foreach (var row in tableArg.Body)
            {
                //table0.AddRow(cells...);
                statements.Add(
                    new CodeMethodInvokeExpression(
                        tableVar,
                        "AddRow",
                        GetStringArrayExpression(row.Cells.Select(c => c.Value), paramToIdentifier)));
            }
            return tableVar;
        }

        private CodeExpression GetMultilineTextArgExpression(string multiLineTextArgument, ParameterSubstitution paramToIdentifier)
        {
            return GetSubstitutedString(multiLineTextArgument, paramToIdentifier);
        }
    }
}
