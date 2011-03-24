using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TechTalk.SpecFlow;
using NUnit.Framework;

namespace TechTalk.SpecFlow.RuntimeTests
{
    [Binding]
    public class StepDefinitionDerived : InheritanceStepsBase
    {
        public StepDefinitionDerived()
        {

        }

        [Then("derived class should have this field already initialized")]
        public void ThenDerivedClassShouldHaveThisFieldAlreadyIniialized()
        {
            Assert.AreEqual(65, a);
        }

        [Given("I have created object of derived class")]
        public void GivenIHaveCreatedObjectOfDerivedClass()
        {

        }
        [When("I change value to (.+) in derived class")]
        public void IChangeValueToXInDerivedClass(int value)
        {
            a = value;
        }

    }
}
