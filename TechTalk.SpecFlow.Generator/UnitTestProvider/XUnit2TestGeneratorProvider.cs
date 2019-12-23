using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Generator.CodeDom;
using BoDi;
using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
    public class XUnit2TestGeneratorProvider : XUnitTestGeneratorProvider
    {
        private new const string THEORY_ATTRIBUTE = "Xunit.TheoryAttribute";
        private const string INLINEDATA_ATTRIBUTE = "Xunit.InlineDataAttribute";
        private const string ICLASSFIXTURE_INTERFACE = "Xunit.IClassFixture";
        private const string COLLECTION_ATTRIBUTE = "Xunit.CollectionAttribute";
        private const string OUTPUT_INTERFACE = "Xunit.Abstractions.ITestOutputHelper";
        private const string OUTPUT_INTERFACE_PARAMETER_NAME = "testOutputHelper";
        private const string OUTPUT_INTERFACE_FIELD_NAME = "_testOutputHelper";
        private const string FIXTUREDATA_PARAMETER_NAME = "fixtureData";
        private const string COLLECTION_TAG = "xunit:collection";
        private static readonly Regex _collectionNameRegex =
            new Regex($@"{COLLECTION_TAG}\(""([^""']+?)""\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private const string COLLECTION_FIXTURE_CLASS_FULL_NAME_TAG = "xunit:collectionFixtureClassFullName";
        private static readonly Regex _collectionFixtureClassFullNameRegex =
            new Regex($@"{COLLECTION_FIXTURE_CLASS_FULL_NAME_TAG}\(""([\w.]+?)""\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private const string COLLECTION_FIXTURE_FIELD_NAME = "_collectionFixture";
        private string _collectionFixtureClassFullName;

        public XUnit2TestGeneratorProvider(CodeDomHelper codeDomHelper)
            :base(codeDomHelper)
        {
            CodeDomHelper = codeDomHelper;
        }

        public override UnitTestGeneratorTraits GetTraits()
        {
            return UnitTestGeneratorTraits.RowTests | UnitTestGeneratorTraits.ParallelExecution;
        }

        protected override CodeTypeReference CreateFixtureInterface(TestClassGenerationContext generationContext, CodeTypeReference fixtureDataType)
        {
            // Add a field for the ITestOutputHelper
            generationContext.TestClass.Members.Add(new CodeMemberField(OUTPUT_INTERFACE, OUTPUT_INTERFACE_FIELD_NAME));

            // Store the fixture data type for later use in constructor
            generationContext.CustomData.Add(FIXTUREDATA_PARAMETER_NAME, fixtureDataType);

            return new CodeTypeReference(ICLASSFIXTURE_INTERFACE, fixtureDataType);
        }

        public override bool ImplmentInterfaceExplicit => false;

        public override void SetRowTest(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle)
        {
            CodeDomHelper.AddAttribute(testMethod, THEORY_ATTRIBUTE, new CodeAttributeArgument("DisplayName", new CodePrimitiveExpression(scenarioTitle)));

            SetProperty(testMethod, FEATURE_TITLE_PROPERTY_NAME, generationContext.Feature.Name);
            SetDescription(testMethod, scenarioTitle);
        }

        public override void SetRow(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> arguments,
            IEnumerable<string> tags, bool isIgnored)
        {
            //TODO: better handle "ignored"
            if (isIgnored)
                return;

            var args = arguments.Select(
                arg => new CodeAttributeArgument(new CodePrimitiveExpression(arg))).ToList();

            args.Add(
                new CodeAttributeArgument(
                    new CodeArrayCreateExpression(typeof(string[]), tags.Select(t => new CodePrimitiveExpression(t)).ToArray())));

            CodeDomHelper.AddAttribute(testMethod, INLINEDATA_ATTRIBUTE, args.ToArray());
        }

        protected override void SetTestConstructor(TestClassGenerationContext generationContext, CodeConstructor ctorMethod) {
            ctorMethod.Parameters.Add(
                new CodeParameterDeclarationExpression((CodeTypeReference)generationContext.CustomData[FIXTUREDATA_PARAMETER_NAME], FIXTUREDATA_PARAMETER_NAME));

            ctorMethod.Parameters.Add(
                new CodeParameterDeclarationExpression(OUTPUT_INTERFACE, OUTPUT_INTERFACE_PARAMETER_NAME));
            ctorMethod.Statements.Add(
                new CodeAssignStatement(
                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), OUTPUT_INTERFACE_FIELD_NAME),
                    new CodeVariableReferenceExpression(OUTPUT_INTERFACE_PARAMETER_NAME)));

            if (_collectionFixtureClassFullName != null)
            {
                const string CollectionFixtureParameterName = "collectionFixture";
                const string CollectionFixtureFieldName = "_collectionFixture";

                ctorMethod.Parameters.Add(
                    new CodeParameterDeclarationExpression(_collectionFixtureClassFullName, CollectionFixtureParameterName));
                generationContext.TestClass.Members.Add(new CodeMemberField(_collectionFixtureClassFullName, CollectionFixtureFieldName));
                ctorMethod.Statements.Add(
                    new CodeAssignStatement(
                        new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), CollectionFixtureFieldName),
                        new CodeVariableReferenceExpression(CollectionFixtureParameterName)));
            }

            base.SetTestConstructor(generationContext, ctorMethod);
        }

        public override void SetTestMethodIgnore(TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
        {
            var factAttr = testMethod.CustomAttributes.OfType<CodeAttributeDeclaration>()
                .FirstOrDefault(codeAttributeDeclaration => codeAttributeDeclaration.Name == FACT_ATTRIBUTE);

            if (factAttr != null)
            {
                // set [FactAttribute(Skip="reason")]
                factAttr.Arguments.Add
                    (
                        new CodeAttributeArgument(FACT_ATTRIBUTE_SKIP_PROPERTY_NAME, new CodePrimitiveExpression(SKIP_REASON))
                    );
            }

            var theoryAttr = testMethod.CustomAttributes.OfType<CodeAttributeDeclaration>()
                .FirstOrDefault(codeAttributeDeclaration => codeAttributeDeclaration.Name == THEORY_ATTRIBUTE);

            if (theoryAttr != null)
            {
                // set [TheoryAttribute(Skip="reason")]
                theoryAttr.Arguments.Add
                    (
                        new CodeAttributeArgument(THEORY_ATTRIBUTE_SKIP_PROPERTY_NAME, new CodePrimitiveExpression(SKIP_REASON))
                    );
            }
        }
        
        public override void SetTestClassCategories(TestClassGenerationContext generationContext, IEnumerable<string> featureCategories)
        {
            var categories = featureCategories.ToArray();
            SetTestClassCollectionIfNeeded(generationContext, categories);
            base.SetTestClassCategories(generationContext, categories);
        }

        private void SetTestClassCollectionIfNeeded(TestClassGenerationContext generationContext, string[] categories)
        {
            var collectionName = FindTagSingleValue(categories, _collectionNameRegex);
            var collectionFixtureClassFullName = FindTagSingleValue(categories, _collectionFixtureClassFullNameRegex);
            if (collectionName == null && collectionFixtureClassFullName == null)
            {
                return;
            }

            if (collectionName != null && collectionFixtureClassFullName != null)
            {
                throw new TestGeneratorException(
                    $"It's not allowed to specify both {COLLECTION_TAG} and {COLLECTION_FIXTURE_CLASS_FULL_NAME_TAG} tags for a feature.");
            }

            if (collectionName != null)
            {
                CodeDomHelper.AddAttribute(
                    generationContext.TestClass,
                    COLLECTION_ATTRIBUTE,
                    new CodeAttributeArgument(new CodePrimitiveExpression(collectionName)));
                return;
            }

            _collectionFixtureClassFullName = collectionFixtureClassFullName;
        }


        private static string FindTagSingleValue(string[] categories, Regex tagMatcher)
        {
            return categories
                   .Select(category => tagMatcher.Match(category))
                   .Where(match => match.Success)
                   .Select(match => match.Value)
                   .SingleOrDefault();
        }

        public override void FinalizeTestClass(TestClassGenerationContext generationContext)
        {
            IgnoreFeature(generationContext);

            // testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<ITestOutputHelper>(_testOutputHelper);
            generationContext.ScenarioInitializeMethod.Statements.Add(
                RegisterInstanceInContainer(generationContext, OUTPUT_INTERFACE, OUTPUT_INTERFACE_FIELD_NAME));

            if (_collectionFixtureClassFullName != null)
            {
                // testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<CollectionFixture>(_collectionFixture);
                generationContext.ScenarioInitializeMethod.Statements.Add(
                    RegisterInstanceInContainer(generationContext, _collectionFixtureClassFullName, COLLECTION_FIXTURE_FIELD_NAME));
            }
        }

        private static CodeMethodInvokeExpression RegisterInstanceInContainer(TestClassGenerationContext generationContext, string type, string fieldName)
        {
            return new CodeMethodInvokeExpression(
                new CodeMethodReferenceExpression(
                    new CodePropertyReferenceExpression(
                        new CodePropertyReferenceExpression(
                            new CodeFieldReferenceExpression(null, generationContext.TestRunnerField.Name),
                            nameof(ScenarioContext)),
                        nameof(ScenarioContext.ScenarioContainer)),
                    nameof(IObjectContainer.RegisterInstanceAs),
                    new CodeTypeReference(type)),
                new CodeVariableReferenceExpression(fieldName));
        }

        protected override bool IsTestMethodAlreadyIgnored(CodeMemberMethod testMethod)
        {
            return IsTestMethodAlreadyIgnored(testMethod, FACT_ATTRIBUTE, THEORY_ATTRIBUTE);
        }
    }
}