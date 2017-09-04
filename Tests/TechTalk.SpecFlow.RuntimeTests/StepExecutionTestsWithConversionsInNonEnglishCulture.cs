using System;
using System.Globalization;
using System.Threading;
using Xunit;
using Rhino.Mocks;
using TechTalk.SpecFlow.Infrastructure;
using TestStatus = TechTalk.SpecFlow.Infrastructure.TestStatus;

namespace TechTalk.SpecFlow.RuntimeTests
{
    [Binding]
    public class StepExecutionTestsBindingsForArgumentConvertInNonEnglishCulture
    {
        [Given("argument (.*) should be able to convert to (.*)")]
        public virtual void InBindingConversion(string doubleParam, double expectedValue)
        {
            double value = Convert.ToDouble(doubleParam);
            Assert.Equal(expectedValue, value);

            Assert.Equal("de-DE", Thread.CurrentThread.CurrentCulture.Name);
        }
    }


    
    public class StepExecutionTestsWithConversionsInNonEnglishCulture : StepExecutionTestsBase
    {
        protected override CultureInfo GetFeatureLanguage()
        {
            return new CultureInfo("de-DE");
        }          
        
        protected override CultureInfo GetBindingCulture()
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

            testRunner.Given("sample step with simple convert param: 1,23"); // German uses , as decimal separator

            Assert.Equal(TestStatus.OK, GetLastTestStatus());
            MockRepository.VerifyAll();
        }

        [Fact]
        public void ShouldExecuteBindingWithTheProperCulture()
        {
            TestRunner testRunner = GetTestRunnerFor(typeof(StepExecutionTestsBindingsForArgumentConvertInNonEnglishCulture));

            MockRepository.ReplayAll();

            testRunner.Given("argument 1,23 should be able to convert to 1,23"); // German uses , as decimal separator

            Assert.Equal(TestStatus.OK, GetLastTestStatus());
            MockRepository.VerifyAll();
        }
    }
}