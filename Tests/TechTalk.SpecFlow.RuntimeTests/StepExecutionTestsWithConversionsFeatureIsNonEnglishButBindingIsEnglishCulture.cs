using System;
using System.Globalization;
using System.Threading;
using FluentAssertions;
using Xunit;
using Rhino.Mocks;
using TechTalk.SpecFlow.Infrastructure;
using ScenarioExecutionStatus = TechTalk.SpecFlow.ScenarioExecutionStatus;

namespace TechTalk.SpecFlow.RuntimeTests
{
    [Binding]
    public class StepExecutionTestsBindingsForArgumentConvertInEnglishCulture
    {
        [Given("argument (.*) should be able to convert to (.*) even though it has english localization")]
        public virtual void InBindingConversion(string doubleParam, double expectedValue)
        {
            double value = Convert.ToDouble(doubleParam);
            Assert.Equal(expectedValue, value);

            Assert.Equal("en-US", Thread.CurrentThread.CurrentCulture.Name);
        }
    }


    
    public class StepExecutionTestsWithConversionsFeatureIsNonEnglishButBindingIsEnglishCulture : StepExecutionTestsBase
    {
        protected override CultureInfo GetFeatureLanguage()
        {
            return new CultureInfo("de-DE");
        }           

        [Fact]
        public void ShouldCallBindingWithSimpleConvertParam()
        {
            StepExecutionTestsBindings bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            bindingInstance.Expect(b => b.BindingWithSimpleConvertParam(1.23));

            MockRepository.ReplayAll();

            testRunner.Given("sample step with simple convert param: 1.23"); // German uses ',' as decimal separator, but BindingCulture is english

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.OK);
            MockRepository.VerifyAll();
        }

        [Fact]
        public void ShouldExecuteBindingWithTheProperCulture()
        {
            TestRunner testRunner = GetTestRunnerFor(typeof(StepExecutionTestsBindingsForArgumentConvertInEnglishCulture));

            MockRepository.ReplayAll();

            testRunner.Given("argument 1.23 should be able to convert to 1.23 even though it has english localization"); // German uses ',' as decimal separator, but BindingCulture is english

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.OK);
            MockRepository.VerifyAll();
        }
    }
}