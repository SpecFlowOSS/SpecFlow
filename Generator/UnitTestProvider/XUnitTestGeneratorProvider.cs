using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
    public class XUnitTestGeneratorProvider : IUnitTestGeneratorProvider
    {
        private const string FEATURE_TITLE_PROPERTY_NAME = "FeatureTitle";
        private const string DESCRIPTION_PROPERTY_NAME = "Description";
        private const string FACT_ATTRIBUTE = "Xunit.FactAttribute";
        private const string FACT_ATTRIBUTE_SKIP_PROPERTY_NAME = "Skip";
        private const string THEORY_ATTRIBUTE = "Xunit.Extensions.TheoryAttribute";
        private const string INLINEDATA_ATTRIBUTE = "Xunit.Extensions.InlineDataAttribute";
        private const string SKIP_REASON = "Ignored";
        private const string TRAIT_ATTRIBUTE = "Xunit.TraitAttribute";
        private const string IUSEFIXTURE_INTERFACE = "Xunit.IUseFixture";

        private CodeTypeDeclaration _currentFixtureDataTypeDeclaration = null;

        protected CodeDomHelper CodeDomHelper { get; set; }

        public bool SupportsRowTests { get { return true; } }
        public bool SupportsAsyncTests { get { return false; } }

        public XUnitTestGeneratorProvider(CodeDomHelper codeDomHelper)
        {
            CodeDomHelper = codeDomHelper;
        }

        public void SetTestClass(TestClassGenerationContext generationContext, string featureTitle, string featureDescription)
        {
            // xUnit does not use an attribute for the TestFixture, all public classes are potential fixtures
        }

        public void SetTestClassCategories(TestClassGenerationContext generationContext, IEnumerable<string> featureCategories)
        {
            // xUnit does not support caregories
        }

        public void SetTestClassInitializeMethod(TestClassGenerationContext generationContext)
        {
            // xUnit uses IUseFixture<T> on the class

            generationContext.TestClassInitializeMethod.Attributes |= MemberAttributes.Static;

            _currentFixtureDataTypeDeclaration = CodeDomHelper.CreateGeneratedTypeDeclaration("FixtureData");
            generationContext.TestClass.Members.Add(_currentFixtureDataTypeDeclaration);

            var fixtureDataType =
                CodeDomHelper.CreateNestedTypeReference(generationContext.TestClass, _currentFixtureDataTypeDeclaration.Name);
            
            var useFixtureType = new CodeTypeReference(IUSEFIXTURE_INTERFACE, fixtureDataType);
            CodeDomHelper.SetTypeReferenceAsInterface(useFixtureType);

            generationContext.TestClass.BaseTypes.Add(useFixtureType);

            // public void SetFixture(T) { } // explicit interface implementation for generic interfaces does not work with codedom

            CodeMemberMethod setFixtureMethod = new CodeMemberMethod();
            setFixtureMethod.Attributes = MemberAttributes.Public;
            setFixtureMethod.Name = "SetFixture";
            setFixtureMethod.Parameters.Add(new CodeParameterDeclarationExpression(fixtureDataType, "fixtureData"));
            setFixtureMethod.ImplementationTypes.Add(useFixtureType);
            generationContext.TestClass.Members.Add(setFixtureMethod);

            // public <_currentFixtureTypeDeclaration>() { <fixtureSetupMethod>(); }
            CodeConstructor ctorMethod = new CodeConstructor();
            ctorMethod.Attributes = MemberAttributes.Public;
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

            CodeMemberMethod disposeMethod = new CodeMemberMethod();
            disposeMethod.PrivateImplementationType = new CodeTypeReference(typeof(IDisposable));
            disposeMethod.Name = "Dispose";
            _currentFixtureDataTypeDeclaration.Members.Add(disposeMethod);

            disposeMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(new CodeTypeReference(generationContext.TestClass.Name)),
                    generationContext.TestClassCleanupMethod.Name));
        }

        public void SetTestMethod(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle)
        {
            CodeDomHelper.AddAttribute(testMethod, FACT_ATTRIBUTE);

            SetProperty(testMethod, FEATURE_TITLE_PROPERTY_NAME, generationContext.Feature.Title);
            SetDescription(testMethod, scenarioTitle);
        }

        public void SetRowTest(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle)
        {
            CodeDomHelper.AddAttribute(testMethod, THEORY_ATTRIBUTE);

            SetProperty(testMethod, FEATURE_TITLE_PROPERTY_NAME, generationContext.Feature.Title);
            SetDescription(testMethod, scenarioTitle);
        }

        public void SetRow(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> arguments, IEnumerable<string> tags, bool isIgnored)
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

        public void SetTestMethodCategories(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> scenarioCategories)
        {
            // xUnit does not support caregories
        }

        public void SetTestInitializeMethod(TestClassGenerationContext generationContext)
        {
            // xUnit uses a parameterless constructor

            // public <_currentTestTypeDeclaration>() { <memberMethod>(); }

            CodeConstructor ctorMethod = new CodeConstructor();
            ctorMethod.Attributes = MemberAttributes.Public;
            generationContext.TestClass.Members.Add(ctorMethod);

            ctorMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    generationContext.TestInitializeMethod.Name));
        }

        public void SetTestCleanupMethod(TestClassGenerationContext generationContext)
        {
            // xUnit supports test tear down through the IDisposable interface

            generationContext.TestClass.BaseTypes.Add(typeof(IDisposable));

            // void IDisposable.Dispose() { <memberMethod>(); }

            CodeMemberMethod disposeMethod = new CodeMemberMethod();
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
            //TODO: how to do class level ignore?
        }

        public void SetTestMethodIgnore(TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
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
        }

        private void SetProperty(CodeTypeMember codeTypeMember, string name, string value)
        {
            CodeDomHelper.AddAttribute(codeTypeMember, TRAIT_ATTRIBUTE, name, value);
        }

        private void SetDescription(CodeTypeMember codeTypeMember, string description)
        {
            // xUnit doesn't have a DescriptionAttribute so using a TraitAttribute instead
            SetProperty(codeTypeMember, DESCRIPTION_PROPERTY_NAME, description);
        }


        public virtual void FinalizeTestClass(TestClassGenerationContext generationContext)
        {
            // by default, doing nothing to the final generated code
        }

        public void SetTestMethodAsRow(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle, string exampleSetName, string variantName, IEnumerable<KeyValuePair<string, string>> arguments)
        {
            // doing nothing since we support RowTest
        }
    }
}