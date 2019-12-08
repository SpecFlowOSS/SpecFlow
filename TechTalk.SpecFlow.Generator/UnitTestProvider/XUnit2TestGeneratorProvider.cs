using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TechTalk.SpecFlow.Generator.CodeDom;
using BoDi;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
    public class XUnit2TestGeneratorProvider : IUnitTestGeneratorProvider
    {
        private readonly ProjectSettings _projectSettings;
        private CodeTypeDeclaration _currentFixtureDataTypeDeclaration = null;
        private readonly CodeTypeReference _objectCodeTypeReference = new CodeTypeReference(typeof(object));
        protected internal const string THEORY_ATTRIBUTE = "Xunit.SkippableTheoryAttribute";
        protected internal const string INLINEDATA_ATTRIBUTE = "Xunit.InlineDataAttribute";
        protected internal const string ICLASSFIXTURE_INTERFACE = "Xunit.IClassFixture";
        protected internal const string COLLECTION_ATTRIBUTE = "Xunit.CollectionAttribute";
        protected internal const string OUTPUT_INTERFACE = "Xunit.Abstractions.ITestOutputHelper";
        protected internal const string OUTPUT_INTERFACE_PARAMETER_NAME = "testOutputHelper";
        protected internal const string OUTPUT_INTERFACE_FIELD_NAME = "_testOutputHelper";
        protected internal const string FIXTUREDATA_PARAMETER_NAME = "fixtureData";
        protected internal const string COLLECTION_DEF = "Xunit.Collection";
        protected internal const string COLLECTION_TAG = "xunit:collection";
        protected internal const string FEATURE_TITLE_PROPERTY_NAME = "FeatureTitle";
        protected internal const string DESCRIPTION_PROPERTY_NAME = "Description";
        protected internal const string FACT_ATTRIBUTE = "Xunit.SkippableFactAttribute";
        protected internal const string FACT_ATTRIBUTE_SKIP_PROPERTY_NAME = "Skip";
        protected internal const string THEORY_ATTRIBUTE_SKIP_PROPERTY_NAME = "Skip";
        protected internal const string SKIP_REASON = "Ignored";
        protected internal const string TRAIT_ATTRIBUTE = "Xunit.TraitAttribute";
        protected internal const string CATEGORY_PROPERTY_NAME = "Category";

        public XUnit2TestGeneratorProvider(CodeDomHelper codeDomHelper, ProjectSettings projectSettings)
        {
            CodeDomHelper = codeDomHelper;
            _projectSettings = projectSettings;
        }

        public virtual void SetTestClass(TestClassGenerationContext generationContext, string featureTitle, string featureDescription)
        {
        }

        public virtual UnitTestGeneratorTraits GetTraits()
        {
            return UnitTestGeneratorTraits.RowTests | UnitTestGeneratorTraits.ParallelExecution;
        }

        protected virtual CodeTypeReference CreateFixtureInterface(TestClassGenerationContext generationContext, CodeTypeReference fixtureDataType)
        {
            // Add a field for the ITestOutputHelper
            generationContext.TestClass.Members.Add(new CodeMemberField(OUTPUT_INTERFACE, OUTPUT_INTERFACE_FIELD_NAME));

            // Store the fixture data type for later use in constructor
            generationContext.CustomData.Add(FIXTUREDATA_PARAMETER_NAME, fixtureDataType);

            return new CodeTypeReference(ICLASSFIXTURE_INTERFACE, fixtureDataType);
        }

        public virtual bool ImplementInterfaceExplicit => false;

        protected CodeDomHelper CodeDomHelper { get; set; }

        public bool GenerateParallelCodeForFeature { get; set; }

        public virtual void SetRowTest(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle)
        {
            CodeDomHelper.AddAttribute(testMethod, THEORY_ATTRIBUTE, new CodeAttributeArgument("DisplayName", new CodePrimitiveExpression(scenarioTitle)));

            SetProperty(testMethod, FEATURE_TITLE_PROPERTY_NAME, generationContext.Feature.Name);
            SetDescription(testMethod, scenarioTitle);
        }

        public virtual void SetRow(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> arguments, IEnumerable<string> tags, bool isIgnored)
        {
            //TODO: better handle "ignored"
            if (isIgnored)
            {
                return;
            }

            var args = arguments.Select(arg => new CodeAttributeArgument(new CodePrimitiveExpression(arg))).ToList();

            args.Add(
                new CodeAttributeArgument(
                    new CodeArrayCreateExpression(typeof(string[]), tags.Select(t => new CodePrimitiveExpression(t)).ToArray())));

            CodeDomHelper.AddAttribute(testMethod, INLINEDATA_ATTRIBUTE, args.ToArray());
        }

        protected virtual void SetTestConstructor(TestClassGenerationContext generationContext, CodeConstructor ctorMethod)
        {
            var typeName = $"{_projectSettings.DefaultNamespace.Replace('.', '_')}_XUnitAssemblyFixture";
            ctorMethod.Parameters.Add(
                new CodeParameterDeclarationExpression((CodeTypeReference)generationContext.CustomData[FIXTUREDATA_PARAMETER_NAME], FIXTUREDATA_PARAMETER_NAME));
            ctorMethod.Parameters.Add(
                new CodeParameterDeclarationExpression(typeName, "assemblyFixture"));
            ctorMethod.Parameters.Add(
                new CodeParameterDeclarationExpression(OUTPUT_INTERFACE, OUTPUT_INTERFACE_PARAMETER_NAME));

            ctorMethod.Statements.Add(
                new CodeAssignStatement(
                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), OUTPUT_INTERFACE_FIELD_NAME),
                    new CodeVariableReferenceExpression(OUTPUT_INTERFACE_PARAMETER_NAME)));

            //ctorMethod.Statements.Add(
            //    new CodeVariableDeclarationStatement(new CodeTypeReference(typeName), "assemblyFixture",
            //        new CodeObjectCreateExpression(new CodeTypeReference(typeName))));

            ctorMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    generationContext.TestInitializeMethod.Name));
        }

        public virtual void SetTestMethodIgnore(TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
        {
            var factAttr = testMethod.CustomAttributes
                                     .OfType<CodeAttributeDeclaration>()
                                     .FirstOrDefault(codeAttributeDeclaration => codeAttributeDeclaration.Name == FACT_ATTRIBUTE);

            // set [FactAttribute(Skip="reason")]
            factAttr?.Arguments.Add(new CodeAttributeArgument(FACT_ATTRIBUTE_SKIP_PROPERTY_NAME, new CodePrimitiveExpression(SKIP_REASON)));

            var theoryAttr = testMethod.CustomAttributes
                                       .OfType<CodeAttributeDeclaration>()
                                       .FirstOrDefault(codeAttributeDeclaration => codeAttributeDeclaration.Name == THEORY_ATTRIBUTE);

            // set [TheoryAttribute(Skip="reason")]
            theoryAttr?.Arguments.Add(new CodeAttributeArgument(THEORY_ATTRIBUTE_SKIP_PROPERTY_NAME, new CodePrimitiveExpression(SKIP_REASON)));
        }
        public virtual void SetTestClassCategories(TestClassGenerationContext generationContext, IEnumerable<string> featureCategories)
        {
            IEnumerable<string> collection = featureCategories.Where(f => f.StartsWith(COLLECTION_TAG, StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (collection.Any())
            {
                //Only one 'Xunit.Collection' can exist per class.
                SetTestClassCollection(generationContext, collection.FirstOrDefault());
            }

            // Set Category trait which can be used with the /trait or /-trait xunit flags to include/exclude tests
            foreach (string str in featureCategories)
            {
                SetProperty(generationContext.TestClass, CATEGORY_PROPERTY_NAME, str);
            }
        }

        public void SetTestClassCollection(TestClassGenerationContext generationContext, string collection)
        {
            //No spaces. 
            //'-', and '_' are allowed.
            string collectionMatch = $@"(?<={COLLECTION_TAG}[(])[A-Za-z0-9\-_]+.*?(?=[)])";
            string description = Regex.Match(collection, collectionMatch, RegexOptions.IgnoreCase).Value;
            CodeDomHelper.AddAttribute(generationContext.TestClass, COLLECTION_DEF, description);
        }


        public virtual void SetTestClassParallelize(TestClassGenerationContext generationContext)
        {
            CodeDomHelper.AddAttribute(generationContext.TestClass, COLLECTION_ATTRIBUTE, new CodeAttributeArgument(new CodePrimitiveExpression(Guid.NewGuid())));
        }

        public virtual void FinalizeTestClass(TestClassGenerationContext generationContext)
        {
            // testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<ITestOutputHelper>(_testOutputHelper);
            generationContext.ScenarioInitializeMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(
                                new CodeFieldReferenceExpression(null, generationContext.TestRunnerField.Name),
                                nameof(ScenarioContext)),
                            nameof(ScenarioContext.ScenarioContainer)),
                        nameof(IObjectContainer.RegisterInstanceAs),
                        new CodeTypeReference(OUTPUT_INTERFACE)),
                    new CodeVariableReferenceExpression(OUTPUT_INTERFACE_FIELD_NAME)));
        }

        protected virtual bool IsTestMethodAlreadyIgnored(CodeMemberMethod testMethod)
        {
            return IsTestMethodAlreadyIgnored(testMethod, FACT_ATTRIBUTE, THEORY_ATTRIBUTE);
        }

        public void SetTestClassInitializeMethod(TestClassGenerationContext generationContext)
        {
            // xUnit uses IUseFixture<T> on the class
            generationContext.TestClassInitializeMethod.Attributes |= MemberAttributes.Static;
            generationContext.TestRunnerField.Attributes |= MemberAttributes.Static;

            _currentFixtureDataTypeDeclaration = CodeDomHelper.CreateGeneratedTypeDeclaration("FixtureData");
            generationContext.TestClass.Members.Add(_currentFixtureDataTypeDeclaration);

            var fixtureDataType =
                CodeDomHelper.CreateNestedTypeReference(generationContext.TestClass, _currentFixtureDataTypeDeclaration.Name);

            var useFixtureType = CreateFixtureInterface(generationContext, fixtureDataType);

            CodeDomHelper.SetTypeReferenceAsInterface(useFixtureType);

            generationContext.TestClass.BaseTypes.Add(_objectCodeTypeReference);
            generationContext.TestClass.BaseTypes.Add(useFixtureType);

            // public <_currentFixtureTypeDeclaration>() { <fixtureSetupMethod>(); }
            var ctorMethod = new CodeConstructor { Attributes = MemberAttributes.Public };
            _currentFixtureDataTypeDeclaration.Members.Add(ctorMethod);
            ctorMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(new CodeTypeReference(generationContext.TestClass.Name)),
                    generationContext.TestClassInitializeMethod.Name));
        }

        public void SetTestClassCleanupMethod(TestClassGenerationContext generationContext)
        {
            // xUnit uses IUseFixture<T> on the class

            generationContext.TestClassCleanupMethod.Attributes |= MemberAttributes.Static;

            _currentFixtureDataTypeDeclaration.BaseTypes.Add(typeof(IDisposable));

            // void IDisposable.Dispose() { <fixtureTearDownMethod>(); }

            var disposeMethod = new CodeMemberMethod();
            disposeMethod.PrivateImplementationType = new CodeTypeReference(typeof(IDisposable));
            disposeMethod.Name = "Dispose";
            _currentFixtureDataTypeDeclaration.Members.Add(disposeMethod);

            disposeMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(new CodeTypeReference(generationContext.TestClass.Name)),
                    generationContext.TestClassCleanupMethod.Name));
        }

        public void SetTestMethod(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string friendlyTestName)
        {
            CodeDomHelper.AddAttribute(testMethod, FACT_ATTRIBUTE, new CodeAttributeArgument("DisplayName", new CodePrimitiveExpression(friendlyTestName)));

            SetProperty(testMethod, FEATURE_TITLE_PROPERTY_NAME, generationContext.Feature.Name);
            SetDescription(testMethod, friendlyTestName);
        }

        public virtual void SetTestMethodCategories(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> scenarioCategories)
        {
            foreach (string str in scenarioCategories)
            {
                SetProperty(testMethod, "Category", str);
            }
        }

        public void SetTestInitializeMethod(TestClassGenerationContext generationContext)
        {
            // xUnit uses a parameterless constructor

            // public <_currentTestTypeDeclaration>() { <memberMethod>(); }

            var ctorMethod = new CodeConstructor();
            ctorMethod.Attributes = MemberAttributes.Public;
            generationContext.TestClass.Members.Add(ctorMethod);

            SetTestConstructor(generationContext, ctorMethod);
        }

        public void SetTestCleanupMethod(TestClassGenerationContext generationContext)
        {
            // xUnit supports test tear down through the IDisposable interface

            generationContext.TestClass.BaseTypes.Add(typeof(IDisposable));

            // void IDisposable.Dispose() { <memberMethod>(); }

            var disposeMethod = new CodeMemberMethod();
            disposeMethod.PrivateImplementationType = new CodeTypeReference(typeof(IDisposable));
            disposeMethod.Name = "Dispose";
            generationContext.TestClass.Members.Add(disposeMethod);

            disposeMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    generationContext.TestCleanupMethod.Name));
        }

        public void SetTestClassIgnore(TestClassGenerationContext generationContext)
        {
            // The individual tests have to get Skip argument in their attributes
            // xUnit does not provide a way to Skip a set of tests - https://xunit.github.io/docs/comparisons.html#attributes
            // This is handled in FinalizeTestClass
        }

        protected void SetProperty(CodeTypeMember codeTypeMember, string name, string value)
        {
            CodeDomHelper.AddAttribute(codeTypeMember, TRAIT_ATTRIBUTE, name, value);
        }

        protected void SetDescription(CodeTypeMember codeTypeMember, string description)
        {
            // xUnit doesn't have a DescriptionAttribute so using a TraitAttribute instead
            SetProperty(codeTypeMember, DESCRIPTION_PROPERTY_NAME, description);
        }

        protected bool IsTestMethodAlreadyIgnored(CodeMemberMethod testMethod, string factAttributeName, string theoryAttributeName)
        {
            var factAttr = testMethod.CustomAttributes.OfType<CodeAttributeDeclaration>()
                                     .FirstOrDefault(codeAttributeDeclaration => codeAttributeDeclaration.Name == factAttributeName);

            var hasIgnoredFact = factAttr?.Arguments
                                         .OfType<CodeAttributeArgument>()
                                         .Any(x => string.Equals(x.Name, FACT_ATTRIBUTE_SKIP_PROPERTY_NAME, StringComparison.InvariantCultureIgnoreCase));

            var theoryAttr = testMethod.CustomAttributes
                                       .OfType<CodeAttributeDeclaration>()
                                       .FirstOrDefault(codeAttributeDeclaration => codeAttributeDeclaration.Name == theoryAttributeName);

            var hasIgnoredTheory = theoryAttr?.Arguments
                                             .OfType<CodeAttributeArgument>()
                                             .Any(x => string.Equals(x.Name, THEORY_ATTRIBUTE_SKIP_PROPERTY_NAME, StringComparison.InvariantCultureIgnoreCase));

            bool result = hasIgnoredFact.GetValueOrDefault() || hasIgnoredTheory.GetValueOrDefault();

            return result;
        }

        public void SetTestMethodAsRow(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle, string exampleSetName, string variantName, IEnumerable<KeyValuePair<string, string>> arguments)
        {
            // doing nothing since we support RowTest
        }
    }
}