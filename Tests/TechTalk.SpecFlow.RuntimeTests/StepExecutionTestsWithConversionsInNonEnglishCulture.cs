using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

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
            return new CultureInfo("de-DE", false);
        }          
        
        protected override CultureInfo GetBindingCulture()
        {
            return new CultureInfo("de-DE", false);
        }     

        [Fact]
        public async Task ShouldCallBindingWithSimpleConvertParam()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsBindings>();

            //bindingInstance.Expect(b => b.BindingWithSimpleConvertParam(1.23));

            //MockRepository.ReplayAll();

            await testRunner.GivenAsync("sample step with simple convert param: 1,23"); // German uses , as decimal separator

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.OK);
            bindingMock.Verify(x => x.BindingWithSimpleConvertParam(1.23));
        }

        [Fact]
        public async Task ShouldExecuteBindingWithTheProperCulture()
        {
            var (testRunner, bindingMock) = GetTestRunnerFor<StepExecutionTestsBindingsForArgumentConvertInNonEnglishCulture>();

            //MockRepository.ReplayAll();

            await testRunner.GivenAsync("argument 1,23 should be able to convert to 1,23"); // German uses , as decimal separator

            GetLastTestStatus().Should().Be(ScenarioExecutionStatus.OK);
            bindingMock.Verify(x => x.InBindingConversion("1,23", 1.23));
        }
    }
}