﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Reflection;
using Moq;
using TechTalk.SpecFlow;

namespace IdeIntegrationTests
{
    [TestFixture]
    public class GoToStepDefinitionTests
    {
        //[TestCase]
        //public void Should_return_null_if_no_match_found()
        //{
        //    var emptyBindings = new List<MethodInfo>();

        //    var input = @"GIVEN unmatched step";
        //    var caretPosition = new CaretPosition(0, input.IndexOf("matched"));
        //    var editorContext = new EditorContext(input, caretPosition);

        //    var helper = new GoToDefinitionHelper(emptyBindings);
        //    var result = helper.GetMethodsMatchingTextAtCaret(editorContext);

        //    Assert.AreEqual(0, result.Count(), "Result is expected to be empty when not bindings exist");
        //}

        //delegate void DummyStepDelegate();

        //[TestCase]
        //public void Should_return_a_single_matching_method_without_arguments()
        //{
        //    DummyStepDelegate @delegate = DummyStepMethod;

        //    var emptyBindings = new List<MethodInfo>()
        //    {
        //        @delegate.Method
        //    };

        //    var input = @"GIVEN dummy step";
        //    var caretPosition = new CaretPosition(0, input.IndexOf("step"));
        //    var editorContext = new EditorContext(input, caretPosition);

        //    var helper = new GoToDefinitionHelper(emptyBindings);
        //    var result = helper.GetMethodsMatchingTextAtCaret(editorContext);

        //    Assert.AreEqual(@delegate.Method, result.Single());
        //}

        //[TestCase]
        //public void Should_return_the_matching_method()
        //{
        //    DummyStepDelegate theMatchingDelegate = DummyStepMethod;
        //    DummyStepDelegate anotherDelegate = AnotherDummyStepMethod;

        //    var emptyBindings = new List<MethodInfo>()
        //    {
        //        theMatchingDelegate.Method, anotherDelegate.Method
        //    };

        //    var input = @"GIVEN dummy step";
        //    var caretPosition = new CaretPosition(0, input.IndexOf("step"));
        //    var editorContext = new EditorContext(input, caretPosition);

        //    var helper = new GoToDefinitionHelper(emptyBindings);
        //    var result = helper.GetMethodsMatchingTextAtCaret(editorContext);

        //    Assert.AreEqual(theMatchingDelegate.Method, result.Single());
        //}

        //[Given("dummy step")]
        //private void DummyStepMethod() {}

        //[Given("another dummy step")]
        //private void AnotherDummyStepMethod() { }
    }
}
