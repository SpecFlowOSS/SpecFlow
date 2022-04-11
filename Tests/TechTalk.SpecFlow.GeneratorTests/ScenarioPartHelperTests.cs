using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.CodeDom;
using TechTalk.SpecFlow.Generator.Generation;
using TechTalk.SpecFlow.Parser;
using System.Globalization;
using System.IO;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.GeneratorTests
{
   
    /// <summary>
    /// 
    /// </summary>
    public class ScenarioPartHelperTests
    {
        /// <summary>
        /// The objective of this test is to prove that SpecFlow Generation will properly handle DocStrings that have parameters that are embedded inside nested angle brackets
        /// </summary>
        [Theory]
        [InlineData(@"Feature: Missing
                            Scenario Outline: Nested angle brackets in outline text
                            Given my XML is
                            """"""
                                <node attrib = ""<Attribute>"" ></node>
                            """""" 
                            When I process XML
                            Then I receive<Result> value

                            Examples:
                            | Attribute | Result |
                            | Foo | Bar | ")]
        [InlineData(@"Feature: Missing
                            Scenario Outline: Nested angle brackets in outline text
                            Given my XML is
                            """"""
                                <node attrib = ""<<<Attribute>>>"" ></node>
                            """""" 
                            When I process XML
                            Then I receive<Result> value

                            Examples:
                            | Attribute | Result |
                            | Foo | Bar | ")]
        public void GenerateStep_ScenarioOutlineWithMultiLineTextThatIncludesParametersEmbeddedInNestedAngleBrackets_GeneratesStepInvocationCodeExpresssionThatIncludesTheParameterValueSubstitutedIntoTheMultiLineParameter(string feature)
        {
            
            var _parser = new SpecFlowGherkinParser(CultureInfo.GetCultureInfo("en"));

            var _specFlowDocument = _parser.Parse(new StringReader(feature), null);
            var _outline = _specFlowDocument.Feature.Children.First().As<ScenarioOutline>();

            var _paramToIdentifier = new ParameterSubstitution();
            foreach (var param in _outline.Examples.First().TableHeader.Cells)
            {
                _paramToIdentifier.Add(param.Value, param.Value.ToIdentifierCamelCase());
            }

            var _givenStep = _outline.Steps.First<Gherkin.Ast.Step>();
       
            CodeDomHelper _codeDomHelper = new CodeDomHelper(CodeDomProviderLanguage.CSharp);
            SpecFlowConfiguration _specFlowConfiguration = ConfigurationLoader.GetDefault(); 
            _specFlowConfiguration.AllowRowTests = true;
            _specFlowConfiguration.AllowDebugGeneratedFiles = true;
            TestClassGenerationContext _generationContext = new TestClassGenerationContext(null, _specFlowDocument, null, null, null, null, null, null, null, null, null, null, null, true);
            List<CodeStatement> _statements = new List<CodeStatement>();
            
            // Creating the SUT
            var _scenarioPartHelper = new ScenarioPartHelper(_specFlowConfiguration, _codeDomHelper);


            // ACT

            _scenarioPartHelper.GenerateStep(_generationContext, _statements, _givenStep, _paramToIdentifier);
      

            //ASSERT

            //That the generated statements includes a Given method invocation
            var _givenStatements = _statements.OfType<CodeExpressionStatement>().Select(ces => ces.Expression).OfType<CodeMethodInvokeExpression>().Where(m => m.Method.MethodName == "Given");
            var _firstGivenStatement = _givenStatements.First();
            _firstGivenStatement.Should().NotBeNull();

            //That the Given method invocation's first parameter is a primitive expression whose value matches the Given statement binding phrase
            var _parameter1GivenStepName = _firstGivenStatement.Parameters[0];
            _parameter1GivenStepName.Should().BeOfType<CodePrimitiveExpression>();
            _parameter1GivenStepName.As<CodePrimitiveExpression>().Value.Should().Be("my XML is");

            //That the Given method invocation includes a second parameter which is a Method Expression
            var _parameter2QuoteString = _firstGivenStatement.Parameters[1];
            _parameter2QuoteString.Should().BeOfType<CodeMethodInvokeExpression>();

            //That Expression invokes String.Format
            var _parameter2QuoteStringMethod = _parameter2QuoteString.As<CodeMethodInvokeExpression>();
            _parameter2QuoteStringMethod.Method.MethodName.Should().Be("Format");

            //That the Expression includes a format string and at least one other paramter
            var _stringFormatMethodParms = _parameter2QuoteStringMethod.Parameters;
            _stringFormatMethodParms.Count.Should().BeGreaterOrEqualTo(2);

            //That the format string includes at least one pair of matching substitution braces {}
            var _formatString = _stringFormatMethodParms[0].As<CodePrimitiveExpression>().Value;
            _formatString.Should().BeOfType<string>();
            var _leftBrackets = _formatString.As<string>().Where(c => c == '{').ToList();
            var _rightBrackets = _formatString.As<string>().Where(c => c == '}').ToList();
            _leftBrackets.Count.Should().BeGreaterOrEqualTo(1);
            _leftBrackets.Count.Should().Be(_rightBrackets.Count);

            //That all the other parameters are variable references whose names match the scenario parameters given in the Examples table
            _stringFormatMethodParms.RemoveAt(0); // removes the format string from the list of parameters to the call to String.Format
            foreach(var p in _stringFormatMethodParms)
            {
                p.Should().BeOfType<CodeVariableReferenceExpression>();
                var _varName = p.As<CodeVariableReferenceExpression>().VariableName;
                _paramToIdentifier.Select(pi => pi.Value).Should().Contain(_varName);
            }          
        }

    }
}
