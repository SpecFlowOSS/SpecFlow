using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.Generator
{
    public class SpecFlowUnitTestConverter : ISpecFlowUnitTestConverter
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
        private readonly CodeDomHelper codeDomHelper;
        private readonly bool allowDebugGeneratedFiles;
        private readonly bool allowRowTests;

        public SpecFlowUnitTestConverter(IUnitTestGeneratorProvider testGeneratorProvider, CodeDomHelper codeDomHelper, bool allowDebugGeneratedFiles, bool allowRowTests)
        {
            this.testGeneratorProvider = testGeneratorProvider;
            this.codeDomHelper = codeDomHelper;
            this.codeDomHelper.InjectIfRequired(this.testGeneratorProvider);
            this.allowDebugGeneratedFiles = allowDebugGeneratedFiles;
            this.allowRowTests = allowRowTests;
        }

        public CodeNamespace GenerateUnitTestFixture(Feature feature, string testClassName, string targetNamespace)
        {
            targetNamespace = targetNamespace ?? DEFAULT_NAMESPACE;
            testClassName = testClassName ?? string.Format(FIXTURE_FORMAT, feature.Title.ToIdentifier());


            CodeNamespace codeNamespace = new CodeNamespace(targetNamespace);

            codeNamespace.Imports.Add(new CodeNamespaceImport(SPECFLOW_NAMESPACE));

            var testType = codeDomHelper.CreateGeneratedTypeDeclaration(testClassName);
            testType.IsPartial = true;
            testType.TypeAttributes |= TypeAttributes.Public;
            codeNamespace.Types.Add(testType);

            AddLinePragmaInitial(testType, feature);

            testGeneratorProvider.SetTestFixture(testType, feature.Title, feature.Description);
            if (feature.Tags != null)
            {
                testGeneratorProvider.SetTestFixtureCategories(testType, GetNonIgnoreTags(feature.Tags));
                if (HasIgnoreTag(feature.Tags))
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
            return codeNamespace;
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
        }

        private IEnumerable<string> GetNonIgnoreTags(IEnumerable<Tag> tags)
        {
            if (tags == null)
                return new string[0];
            return tags.Where(t => !t.Name.Equals(IGNORE_TAG, StringComparison.InvariantCultureIgnoreCase)).Select(t => t.Name);
        }

        private bool HasIgnoreTag(IEnumerable<Tag> tags)
        {
            if (tags == null)
                return false;
            return tags.Any(t => t.Name.Equals(IGNORE_TAG, StringComparison.InvariantCultureIgnoreCase));
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
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression("GenerationTargetLanguage"),
                            codeDomHelper.TargetLanguage.ToString()),
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

            return new CodeArrayCreateExpression(typeof(string[]), items.ToArray());
        }

        private CodeExpression GetStringArrayExpression(IEnumerable<string> items, ParameterSubstitution paramToIdentifier)
        {
            List<CodeExpression> expressions = new List<CodeExpression>();
            foreach (var item in items)
            {
                expressions.Add(GetSubstitutedString(item, paramToIdentifier));
            }

            return new CodeArrayCreateExpression(typeof(string[]), expressions.ToArray());
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

            AddLineDirective(backgroundMethod.Statements, background);

            foreach (var given in background.Steps)
                GenerateStep(backgroundMethod, given, null);

            AddLineDirectiveHidden(backgroundMethod.Statements);

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

            var testMethod = GenerateScenarioOutlineBody(feature, scenarioOutline, paramToIdentifier, testType, testMethodName, testSetup);

            if (testGeneratorProvider.SupportsRowTests && this.allowRowTests)
            {
                SetTestMethodDeclaration(testMethod, scenarioOutline, null, rowTest: true);

                foreach (var exampleSet in scenarioOutline.Examples.ExampleSets)
                {
                    for (int rowIndex = 0; rowIndex < exampleSet.Table.Body.Length; rowIndex++)
                    {
                        //TODO: handle examples tags
                        IEnumerable<string> arguments = exampleSet.Table.Body[rowIndex].Cells.Select(c => c.Value);
                        testGeneratorProvider.SetRow(testMethod, arguments, GetNonIgnoreTags(exampleSet.Tags), HasIgnoreTag(exampleSet.Tags));
                    }
                }
            }
            else
            {
                int exampleSetIndex = 0;
                foreach (var exampleSet in scenarioOutline.Examples.ExampleSets)
                {
                    string exampleSetTitle = exampleSet.Title == null
                        ? string.Format("Scenarios{0}", exampleSetIndex + 1)
                        : exampleSet.Title.ToIdentifier();

                    bool useFirstColumnAsName = CanUseFirstColumnAsName(exampleSet.Table);

                    for (int rowIndex = 0; rowIndex < exampleSet.Table.Body.Length; rowIndex++)
                    {
                        string variantName = useFirstColumnAsName ? exampleSet.Table.Body[rowIndex].Cells[0].Value.ToIdentifierPart() :
                                                                                                                                          string.Format("Variant{0}", rowIndex);
                        GenerateScenarioOutlineTestVariant(testType, scenarioOutline, testMethodName, paramToIdentifier, exampleSetTitle, exampleSet.Table.Body[rowIndex], exampleSet.Tags, variantName);
                    }
                    exampleSetIndex++;
                }
            }
        }

        private bool CanUseFirstColumnAsName(Table table)
        {
            if (table.Header.Cells.Length == 0)
                return false;

            return table.Body.Select(r => r.Cells[0].Value.ToIdentifier()).Distinct().Count() == table.Body.Length;
        }

        private CodeMemberMethod GenerateScenarioOutlineBody(Feature feature, ScenarioOutline scenarioOutline, ParameterSubstitution paramToIdentifier, CodeTypeDeclaration testType, string testMethodName, CodeMemberMethod testSetup)
        {
            CodeMemberMethod testMethod = new CodeMemberMethod();
            testType.Members.Add(testMethod);

            testMethod.Attributes = MemberAttributes.Public;
            testMethod.Name = testMethodName;

            foreach (var pair in paramToIdentifier)
            {
                testMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), pair.Value));
            }

            testMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof (string[]), "exampleTags"));
            var exampleTagsExpression = new CodeVariableReferenceExpression("exampleTags");

            GenerateTestBody(feature, scenarioOutline, exampleTagsExpression, testMethod, testSetup, paramToIdentifier);

            return testMethod;
        }

        private void GenerateScenarioOutlineTestVariant(CodeTypeDeclaration testType, ScenarioOutline scenarioOutline, string testMethodName, List<KeyValuePair<string, string>> paramToIdentifier, string exampleSetTitle, Row row, Tags exampleSetTags, string variantName)
        {
            CodeMemberMethod testMethod = GetTestMethodDeclaration(testType, scenarioOutline, exampleSetTags);
            testMethod.Name = string.IsNullOrEmpty(exampleSetTitle)
                ? string.Format("{0}_{1}", testMethod.Name, variantName)
                : string.Format("{0}_{1}_{2}", testMethod.Name, exampleSetTitle, variantName);

            //call test implementation with the params
            List<CodeExpression> argumentExpressions = new List<CodeExpression>();
            foreach (var paramCell in row.Cells)
            {
                argumentExpressions.Add(new CodePrimitiveExpression(paramCell.Value));
            }

            argumentExpressions.Add(GetStringArrayExpression(exampleSetTags));

            testMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    testMethodName,
                    argumentExpressions.ToArray()));
        }

        private void GenerateTest(CodeTypeDeclaration testType, CodeMemberMethod testSetup, Scenario scenario, Feature feature)
        {
            CodeMemberMethod testMethod = GetTestMethodDeclaration(testType, scenario, null);
            GenerateTestBody(feature, scenario, null, testMethod, testSetup, null);
        }

        private void GenerateTestBody(Feature feature, Scenario scenario, CodeExpression additionalTagsExpression, CodeMemberMethod testMethod, CodeMemberMethod testSetup, ParameterSubstitution paramToIdentifier)
        {
            //call test setup
            //ScenarioInfo scenarioInfo = new ScenarioInfo("xxxx", tags...);
            CodeExpression tagsExpression;
            if (additionalTagsExpression == null)
                tagsExpression = GetStringArrayExpression(scenario.Tags);
            else if (scenario.Tags == null)
                tagsExpression = additionalTagsExpression;
            else
            {
                // merge tags list
                // Enumerable.ToArray(Enumerable.Concat(tags1, tags1));
                tagsExpression = new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(typeof(Enumerable)),
                    "ToArray",
                    new CodeMethodInvokeExpression(
                        new CodeTypeReferenceExpression(typeof(Enumerable)),
                        "Concat",
                        GetStringArrayExpression(scenario.Tags),
                        additionalTagsExpression));
            }
            testMethod.Statements.Add(
                new CodeVariableDeclarationStatement(SCENARIOINFO_TYPE, "scenarioInfo",
                    new CodeObjectCreateExpression(SCENARIOINFO_TYPE,
                        new CodePrimitiveExpression(scenario.Title),
                        tagsExpression)));

            AddLineDirective(testMethod.Statements, scenario);
            testMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    testSetup.Name,
                    new CodeVariableReferenceExpression("scenarioInfo")));

            foreach (var scenarioStep in scenario.Steps)
            {
                GenerateStep(testMethod, scenarioStep, paramToIdentifier);
            }

            AddLineDirectiveHidden(testMethod.Statements);

            // call collect errors
            var testRunnerField = GetTestRunnerExpression();
            //testRunner.CollectScenarioErrors();
            testMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    testRunnerField,
                    "CollectScenarioErrors"));

        }

        private CodeMemberMethod GetTestMethodDeclaration(CodeTypeDeclaration testType, Scenario scenario, Tags additionalTags)
        {
            CodeMemberMethod testMethod = new CodeMemberMethod();
            testType.Members.Add(testMethod);

            SetTestMethodDeclaration(testMethod, scenario, additionalTags);

            return testMethod;
        }

        private void SetTestMethodDeclaration(CodeMemberMethod testMethod, Scenario scenario, Tags additionalTags, bool rowTest = false)
        {
            testMethod.Attributes = MemberAttributes.Public;
            testMethod.Name = string.Format(TEST_FORMAT, scenario.Title.ToIdentifier());

            if (rowTest)
                testGeneratorProvider.SetRowTest(testMethod, scenario.Title);
            else
                testGeneratorProvider.SetTest(testMethod, scenario.Title);

            if (scenario.Tags != null)
                SetCategoriesFromTags(testMethod, scenario.Tags);
            if (additionalTags != null)
                SetCategoriesFromTags(testMethod, additionalTags);
        }

        private void SetCategoriesFromTags(CodeMemberMethod testMethod, Tags tags)
        {
            testGeneratorProvider.SetTestCategories(testMethod, GetNonIgnoreTags(tags));
            if (HasIgnoreTag(tags))
                testGeneratorProvider.SetIgnore(testMethod);
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
                new CodeTypeReferenceExpression(typeof(string)),
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
                AddLineDirectiveHidden(testMethod.Statements);
                arguments.Add(
                    GetMultilineTextArgExpression(scenarioStep.MultiLineTextArgument, paramToIdentifier));
                arguments.Add(
                    GetTableArgExpression(scenarioStep.TableArg, testMethod.Statements, paramToIdentifier));
            }

            AddLineDirective(testMethod.Statements, scenarioStep);
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

        #region Line pragma handling

        private void AddLinePragmaInitial(CodeTypeDeclaration testType, Feature feature)
        {
            if (allowDebugGeneratedFiles)
                return;

            codeDomHelper.BindTypeToSourceFile(testType, Path.GetFileName(feature.SourceFile));
        }

        private void AddLineDirectiveHidden(CodeStatementCollection statements)
        {
            if (allowDebugGeneratedFiles)
                return;

            codeDomHelper.AddDisableSourceLinePragmaStatement(statements);
        }

        private void AddLineDirective(CodeStatementCollection statements, Background background)
        {
            AddLineDirective(statements, background.FilePosition);
        }

        private void AddLineDirective(CodeStatementCollection statements, Scenario scenario)
        {
            AddLineDirective(statements, scenario.FilePosition);
        }

        private void AddLineDirective(CodeStatementCollection statements, ScenarioStep step)
        {
            AddLineDirective(statements, step.FilePosition);
        }

        private void AddLineDirective(CodeStatementCollection statements, FilePosition filePosition)
        {
            if (filePosition == null || allowDebugGeneratedFiles)
                return;

            codeDomHelper.AddSourceLinePragmaStatement(statements, filePosition.Line, filePosition.Column);
        }

        #endregion
    }
}
