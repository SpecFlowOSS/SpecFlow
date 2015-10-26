using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Assist.ValueRetrievers;
using Moq;
using TechTalk.SpecFlow.Bindings;
using System.Collections.Generic;
using System.Globalization;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class StepTransformationValueRetrieverTests
    {
        [Test]
        public void Returns_the_string_value_back()
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

            var hit = stepArgumentTypeConverter.Convert ("testValue", typeof(string), cultureInfo);

            var result = retriever.Retrieve(new System.Collections.Generic.KeyValuePair<string, string>("", "test"), typeof(string));

        }
    }
}