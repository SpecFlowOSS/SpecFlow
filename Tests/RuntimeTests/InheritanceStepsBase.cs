using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;
using NUnit.Framework;

namespace TechTalk.SpecFlow.RuntimeTests
{
    public abstract class InheritanceStepsBase
    {
        protected int a = 0;

        public InheritanceStepsBase()
        {
            a = 65;
        }

        [Given("base constructor initializes class protected field")]
        public void GivenBaseConstructorInitializesClassProtectedField()
        {
        }
        [Then("field in base class should also contain (.+)")]
        public void ThenFieldInBaseClassShouldAlsoContainX(int expected)
        {
            Assert.AreEqual(expected, a);
        }

    }
}
