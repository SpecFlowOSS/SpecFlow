using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Parser.Gherkin;

// ReSharper disable CollectionNeverUpdated.Local
// ReSharper disable StringIndexOfIsCultureSpecific.1

namespace Vs2010IntegrationUnitTests
{
    [TestFixture]
    public class GoToStepDefinitionTests
    {
        [TestCase]
        public void Should_return_null_if_no_match_found()
        {
            var emptyBindings = new List<MethodInfo>();

            var input = CreateScenario("Given unmatched step");
            var caretPosition = new CaretPosition(0, input.IndexOf("matched"));
            var editor = new GherkinBuffer(input);

            var helper = new GoToDefinitionHelper(emptyBindings);
            var result = helper.GetMethodsMatchingTextAtCaret(editor, caretPosition);

            Assert.AreEqual(0, result.Count(), "Result is expected to be empty when not bindings exist");
        }

        private static string CreateScenario(string scenarioBody)
        {
            return
@"Feature: dummy Feature
Scenario: dummy scenario
" + scenarioBody;
        }

        delegate void DummyStepDelegate();

        [TestCase]
        public void Should_return_a_single_matching_method_without_arguments()
        {
            DummyStepDelegate @delegate = DummyBindingClass.DummyStepMethod;

            var emptyBindings = new List<MethodInfo>()
            {
                @delegate.Method
            };

            var input = CreateScenario("Given dummy step");
            var caretPosition = FindWordPosition(input, "step");
            var editor = new GherkinBuffer(input);

            var helper = new GoToDefinitionHelper(emptyBindings);
            var result = helper.GetMethodsMatchingTextAtCaret(editor, caretPosition);

            Assert.AreEqual(1, result.Count(), "Number of matching methods in result");
            Assert.AreEqual(@delegate.Method, result.Single(), "MethodInfo");
        }

        private static CaretPosition FindWordPosition(string input, string word)
        {
            //// TODO: can't this method be simplified?!
            //var tempContext = new EditorContext(input, new CaretPosition(0, 0));
            var absolutePosition = input.IndexOf(word);
            //var line = tempContext.GetLineNumberFromPosition(absolutePosition);
            //var linePosition = tempContext.GetAbsolutePositionFromLine(line);
            //return new CaretPosition(line, absolutePosition - linePosition);
            var gherkinBuffer = new GherkinBuffer(input);
            var line = gherkinBuffer.GetLineNumberFromPosition(absolutePosition);
            var linePosition = gherkinBuffer.GetBufferPositionFromLine(line);
            return new CaretPosition(line, absolutePosition - linePosition);
        }

        [TestCase]
        public void Should_return_the_matching_method()
        {
            DummyStepDelegate theMatchingDelegate = DummyBindingClass.DummyStepMethod;
            DummyStepDelegate anotherDelegate = DummyBindingClass.AnotherDummyStepMethod;

            var emptyBindings = new List<MethodInfo>()
            {
                theMatchingDelegate.Method, anotherDelegate.Method
            };

            var input = CreateScenario("Given dummy step");
            var caretPosition = new CaretPosition(0, input.IndexOf("step"));
            var editor = new GherkinBuffer(input);

            var helper = new GoToDefinitionHelper(emptyBindings);
            var result = helper.GetMethodsMatchingTextAtCaret(editor, caretPosition);

            Assert.AreEqual(theMatchingDelegate.Method, result.Single());
        }

        [Binding]
        public class DummyBindingClass
        {
            [Given("dummy step")]
            public static void DummyStepMethod()
            {
            }

            [Given("another dummy step")]
            public static void AnotherDummyStepMethod()
            {
            }
        }
    }
}
