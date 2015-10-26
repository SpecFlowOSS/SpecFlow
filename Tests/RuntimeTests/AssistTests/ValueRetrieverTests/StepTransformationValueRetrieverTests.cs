using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Assist.ValueRetrievers;
using Moq;
using TechTalk.SpecFlow.Bindings;
using System.Collections.Generic;
using System.Globalization;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;
using System;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class StepTransformationValueRetrieverTests
    {
        [Test]
        public void Convert_will_return_the_value_from_the_step_argument_type_converter()
        {
            var retriever = new StepTransformationValueRetriever();

            var container = new BoDi.ObjectContainer();
            var provider = new TechTalk.SpecFlow.Infrastructure.DefaultDependencyProvider();
            provider.RegisterTestRunnerDefaults(container);

            Mock<IBindingRegistry> bindingRegistryStub = new Mock<IBindingRegistry>();
            List<IStepArgumentTransformationBinding> stepTransformations = new List<IStepArgumentTransformationBinding>();
            bindingRegistryStub.Setup(br => br.GetStepTransformations()).Returns(stepTransformations);

            Mock<IBindingInvoker> methodBindingInvokerStub = new Mock<IBindingInvoker>();

            var stepArgumentTypeConverter = new StepArgumentTypeConverter(new Mock<ITestTracer>().Object, bindingRegistryStub.Object, new Mock<IContextManager>().Object, methodBindingInvokerStub.Object);
            var cultureInfo = new CultureInfo("en-US");

            container.RegisterInstanceAs<IStepArgumentTypeConverter>(stepArgumentTypeConverter);
            container.RegisterInstanceAs<CultureInfo>(cultureInfo);

            var hit = stepArgumentTypeConverter.Convert("testValue", typeof(string), cultureInfo);
            retriever.Container = container;

            var result = retriever.Retrieve(new System.Collections.Generic.KeyValuePair<string, string>("", "2009/10/06"), typeof(DateTime));

            result.Should().Be(new DateTime(2009, 10, 6));
        }
    }
}