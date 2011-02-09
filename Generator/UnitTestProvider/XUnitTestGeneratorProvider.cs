using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
    public class XUnitTestGeneratorProvider : IUnitTestGeneratorProvider, ICodeDomHelperRequired
    {
        private const string FEATURE_TITLE_KEY = "FeatureTitle";
        private const string FEATURE_TITLE_PROPERTY_NAME = "FeatureTitle";
        private const string FACT_ATTRIBUTE = "Xunit.FactAttribute";
        private const string FACT_ATTRIBUTE_SKIP_PROPERTY_NAME = "Skip";
        private const string THEORY_ATTRIBUTE = "Xunit.TheoryAttribute";
        private const string INLINEDATA_ATTRIBUTE = "Xunit.InlineDataAttribute";
        private const string SKIP_REASON = "Ignored";
        private const string TRAIT_ATTRIBUTE = "Xunit.TraitAttribute";
        private const string IUSEFIXTURE_INTERFACE = "Xunit.IUseFixture";

        private CodeTypeDeclaration _currentTestTypeDeclaration = null;
        private CodeTypeDeclaration _currentFixtureTypeDeclaration = null;

        public CodeDomHelper CodeDomHelper { get; set; }

        public bool SupportsRowTests { get { return true; } }

        public void SetTestFixture(CodeTypeDeclaration typeDeclaration, string title, string description)
        {
            // xUnit does not use an attribute for the TestFixture, all public classes are potential fixtures

            // Remember the feature title for use later
            typeDeclaration.UserData[FEATURE_TITLE_KEY] = title;

            _currentTestTypeDeclaration = typeDeclaration;
        }

        public void SetTestFixtureCategories(CodeTypeDeclaration typeDeclaration, IEnumerable<string> categories)
        {
            // xUnit does not support caregories
        }

        public void SetTestFixtureSetup(CodeMemberMethod fixtureSetupMethod)
        {
            // xUnit uses IUseFixture<T> on the class

            fixtureSetupMethod.Attributes |= MemberAttributes.Static;

            _currentFixtureTypeDeclaration = CodeDomHelper.CreateGeneratedTypeDeclaration("FixtureData");
            _currentTestTypeDeclaration.Members.Add(_currentFixtureTypeDeclaration);

            var fixtureDataType = 
                CodeDomHelper.CreateNestedTypeReference(_currentTestTypeDeclaration, _currentFixtureTypeDeclaration.Name);
            
            var useFixtureType = new CodeTypeReference(IUSEFIXTURE_INTERFACE, fixtureDataType);
            CodeDomHelper.SetTypeReferenceAsInterface(useFixtureType);

            _currentTestTypeDeclaration.BaseTypes.Add(useFixtureType);

            // public void SetFixture(T) { } // explicit interface implementation for generic interfaces does not work with codedom

            CodeMemberMethod setFixtureMethod = new CodeMemberMethod();
            setFixtureMethod.Attributes = MemberAttributes.Public;
            setFixtureMethod.Name = "SetFixture";
            setFixtureMethod.Parameters.Add(new CodeParameterDeclarationExpression(fixtureDataType, "fixtureData"));
            setFixtureMethod.ImplementationTypes.Add(useFixtureType);
            _currentTestTypeDeclaration.Members.Add(setFixtureMethod);

            // public <_currentFixtureTypeDeclaration>() { <fixtureSetupMethod>(); }
            CodeConstructor ctorMethod = new CodeConstructor();
            ctorMethod.Attributes = MemberAttributes.Public;
            _currentFixtureTypeDeclaration.Members.Add(ctorMethod);
            ctorMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(new CodeTypeReference(_currentTestTypeDeclaration.Name)),
                    fixtureSetupMethod.Name));
        }

        public void SetTestFixtureTearDown(CodeMemberMethod fixtureTearDownMethod)
        {
            // xUnit uses IUseFixture<T> on the class

            fixtureTearDownMethod.Attributes |= MemberAttributes.Static;

            _currentFixtureTypeDeclaration.BaseTypes.Add(typeof(IDisposable));

            // void IDisposable.Dispose() { <fixtureTearDownMethod>(); }

            CodeMemberMethod disposeMethod = new CodeMemberMethod();
            disposeMethod.PrivateImplementationType = new CodeTypeReference(typeof(IDisposable));
            disposeMethod.Name = "Dispose";
            _currentFixtureTypeDeclaration.Members.Add(disposeMethod);

            disposeMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(new CodeTypeReference(_currentTestTypeDeclaration.Name)),
                    fixtureTearDownMethod.Name));
        }

        public void SetTest(CodeMemberMethod memberMethod, string title)
        {
            memberMethod.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(FACT_ATTRIBUTE)));

            if (_currentTestTypeDeclaration != null)
            {
                string featureTitle = _currentTestTypeDeclaration.UserData[FEATURE_TITLE_KEY] as string;
                if (!string.IsNullOrEmpty(featureTitle))
                {
                    SetProperty(memberMethod.CustomAttributes, FEATURE_TITLE_PROPERTY_NAME, featureTitle);
                }
            }

            SetDescription(memberMethod.CustomAttributes, title);
        }

        public void SetRowTest(CodeMemberMethod memberMethod, string title)
        {
            memberMethod.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(THEORY_ATTRIBUTE)));

            if (_currentTestTypeDeclaration != null)
            {
                string featureTitle = _currentTestTypeDeclaration.UserData[FEATURE_TITLE_KEY] as string;
                if (!string.IsNullOrEmpty(featureTitle))
                {
                    SetProperty(memberMethod.CustomAttributes, FEATURE_TITLE_PROPERTY_NAME, featureTitle);
                }
            }

            SetDescription(memberMethod.CustomAttributes, title);
        }

        public void SetRow(CodeMemberMethod memberMethod, IEnumerable<string> arguments, IEnumerable<string> tags, bool isIgnored)
        {
            //TODO: better handle "ignored"
            if (isIgnored)
                return;

            var args = arguments.Select(
              arg => new CodeAttributeArgument(new CodePrimitiveExpression(arg))).ToList();

            args.Add(
                new CodeAttributeArgument(
                    new CodeArrayCreateExpression(typeof(string[]), tags.Select(t => new CodePrimitiveExpression(t)).ToArray())));

            memberMethod.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(INLINEDATA_ATTRIBUTE),
                    args.ToArray()));
        }

        public void     SetTestCategories(CodeMemberMethod memberMethod, IEnumerable<string> categories)
        {
            // xUnit does not support caregories
        }

        public void SetTestSetup(CodeMemberMethod memberMethod)
        {
            // xUnit uses a parameterless constructor

            // public <_currentTestTypeDeclaration>() { <memberMethod>(); }

            CodeConstructor ctorMethod = new CodeConstructor();
            ctorMethod.Attributes = MemberAttributes.Public;
            _currentTestTypeDeclaration.Members.Add(ctorMethod);

            ctorMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    memberMethod.Name));
        }

        public void SetTestTearDown(CodeMemberMethod memberMethod)
        {
            // xUnit supports test tear down through the IDisposable interface

            _currentTestTypeDeclaration.BaseTypes.Add(typeof(IDisposable));

            // void IDisposable.Dispose() { <memberMethod>(); }

            CodeMemberMethod disposeMethod = new CodeMemberMethod();
            disposeMethod.PrivateImplementationType = new CodeTypeReference(typeof(IDisposable));
            disposeMethod.Name = "Dispose";
            _currentTestTypeDeclaration.Members.Add(disposeMethod);

            disposeMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    memberMethod.Name));
        }

        public void SetIgnore(CodeTypeMember codeTypeMember)
        {
            foreach (var customAttribute in codeTypeMember.CustomAttributes)
            {
                CodeAttributeDeclaration codeAttributeDeclaration = customAttribute as CodeAttributeDeclaration;
                if (codeAttributeDeclaration != null && codeAttributeDeclaration.Name == FACT_ATTRIBUTE)
                {
                    // set [FactAttribute(Skip="reason")]
                    codeAttributeDeclaration.Arguments.Add
                    (
                        new CodeAttributeArgument(FACT_ATTRIBUTE_SKIP_PROPERTY_NAME, new CodePrimitiveExpression(SKIP_REASON))
                    );
                    break;
                }
            }
        }

        private void SetProperty(CodeAttributeDeclarationCollection customAttributes, string name, string value)
        {
            customAttributes.Add
            (
                new CodeAttributeDeclaration
                (
                    new CodeTypeReference(TRAIT_ATTRIBUTE),
                    new CodeAttributeArgument
                    (
                        new CodePrimitiveExpression(name)
                    ),
                    new CodeAttributeArgument
                    (
                        new CodePrimitiveExpression(value)
                    )
                )
            );
        }

        private void SetDescription(CodeAttributeDeclarationCollection customAttributes, string description)
        {
            // xUnit doesn't have a DescriptionAttribute so using a TraitAttribute instead
            SetProperty(customAttributes, "Description", description);
        }
    }
}