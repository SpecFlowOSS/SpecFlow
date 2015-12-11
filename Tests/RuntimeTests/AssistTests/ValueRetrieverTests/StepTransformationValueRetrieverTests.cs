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
using TechTalk.SpecFlow.Bindings.Reflection;
using BoDi;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class StepTransformationValueRetrieverTests
    {

        [Test]
        public void Retrieve_will_return_the_value_from_the_step_argument_type_converter()
        {
            var dateTimeResult = Subject().Retrieve(KeyValueFor("2009/10/06"), typeof(DateTime));
            dateTimeResult.Should().Be(new DateTime(2009, 10, 6));

            var stringResult = Subject().Retrieve(KeyValueFor("2009/10/06"), typeof(string));
            stringResult.Should().Be("2009/10/06");
        }

        [Test]
        public void Retriever_will_use_the_current_culture_info()
        {
            var stepArgumentTypeConverter = new Mock<IStepArgumentTypeConverter>();
            var value = Guid.NewGuid().ToString();

            var containerToUseForThePurposeOfTesting = BuildAUseableContainerForTesting();

            //one culture
            var frenchCultureInfo = new CultureInfo("fr-FR");
            containerToUseForThePurposeOfTesting.RegisterInstanceAs<IStepArgumentTypeConverter>(stepArgumentTypeConverter.Object);
            RegisterBindingCulture(frenchCultureInfo, containerToUseForThePurposeOfTesting);

            var french = new Object();
            stepArgumentTypeConverter.Setup(x => x.Convert(value, It.IsAny<IBindingType>(), frenchCultureInfo)).Returns(french);
            Subject(containerToUseForThePurposeOfTesting).Retrieve(KeyValueFor(value), typeof(DateTime)).Should().BeSameAs(french);

            //another culture
            var anotherCultureContainer = BuildAUseableContainerForTesting();

            var usCultureInfo = new CultureInfo("fr-FR");
            anotherCultureContainer.RegisterInstanceAs<IStepArgumentTypeConverter>(stepArgumentTypeConverter.Object);
            RegisterBindingCulture(usCultureInfo, anotherCultureContainer);

            var us = new Object();
            stepArgumentTypeConverter.Setup(x => x.Convert(value, It.IsAny<IBindingType>(), usCultureInfo)).Returns(us);
            Subject(anotherCultureContainer).Retrieve(KeyValueFor(value), typeof(DateTime)).Should().BeSameAs(us);
        }

        [Test]
        public void CanRetrieve_will_return_true_if_the_value_can_be_retrieved_from_a_step_argument_transformation()
        {
            Subject().CanRetrieve(KeyValueFor("2009/10/06"), typeof(DateTime)).Should().BeTrue();
            Subject().CanRetrieve(KeyValueFor("not a date"), typeof(DateTime)).Should().BeFalse();
            Subject().CanRetrieve(KeyValueFor("not a date"), typeof(string)).Should().BeTrue();
        }

        //TODO[assistcont]: this test does not make sense anymore because StepTransformationValueRetriever will not even registered when there is not current context. We should replace this with a test in ServiceTests
        [Test]
        public void CanRetrieve_will_return_false_if_the_step_argument_transformation_work_is_throwing()
        {
            var containerToUseForThePurposeOfTesting = BuildAUseableContainerForTesting();

            // setup IStepArgumentTypeConverter to thorw exceptions
            var stepArgumentTypeConverter = new Mock<IStepArgumentTypeConverter>();
            stepArgumentTypeConverter.Setup(c => c.CanConvert(It.IsAny<object>(), It.IsAny<IBindingType>(), It.IsAny<CultureInfo>()))
                .Throws<Exception>();
            stepArgumentTypeConverter.Setup(c => c.Convert(It.IsAny<object>(), It.IsAny<IBindingType>(), It.IsAny<CultureInfo>()))
                .Throws<Exception>();
            containerToUseForThePurposeOfTesting.RegisterInstanceAs<IStepArgumentTypeConverter>(stepArgumentTypeConverter.Object);

            var subject = Subject(containerToUseForThePurposeOfTesting);

            subject.CanRetrieve(KeyValueFor("2009/10/06"), typeof(DateTime)).Should().BeFalse();
            subject.CanRetrieve(KeyValueFor("not a date"), typeof(DateTime)).Should().BeFalse();
            subject.CanRetrieve(KeyValueFor("not a date"), typeof(string)).Should().BeFalse();
        }

        [Test]
        public void CanRetriever_will_use_the_current_culture_info()
        {
            var containerToUseForThePurposeOfTesting = BuildAUseableContainerForTesting();

            //one culture
            var frenchCultureInfo = new CultureInfo("fr-FR");
            var stepArgumentTypeConverter = new Mock<IStepArgumentTypeConverter>();
            containerToUseForThePurposeOfTesting.RegisterInstanceAs<IStepArgumentTypeConverter>(stepArgumentTypeConverter.Object);
            RegisterBindingCulture(frenchCultureInfo, containerToUseForThePurposeOfTesting);

            stepArgumentTypeConverter.Setup(x => x.CanConvert("2009/10/06", It.IsAny<IBindingType>(), frenchCultureInfo)).Returns(true);
            Subject(containerToUseForThePurposeOfTesting).CanRetrieve(KeyValueFor("2009/10/06"), typeof(DateTime)).Should().BeTrue();

            stepArgumentTypeConverter.Setup(x => x.CanConvert("2009/10/06", It.IsAny<IBindingType>(), frenchCultureInfo)).Returns(false);
            Subject(containerToUseForThePurposeOfTesting).CanRetrieve(KeyValueFor("2009/10/06"), typeof(DateTime)).Should().BeFalse();

            //another culture
            var anotherCultureContainer = BuildAUseableContainerForTesting();
            var usCultureInfo = new CultureInfo("en-US");
            anotherCultureContainer.RegisterInstanceAs<IStepArgumentTypeConverter>(stepArgumentTypeConverter.Object);
            RegisterBindingCulture(usCultureInfo, anotherCultureContainer);

            stepArgumentTypeConverter.Setup(x => x.CanConvert("2009/10/06", It.IsAny<IBindingType>(), usCultureInfo)).Returns(true);
            Subject(anotherCultureContainer).CanRetrieve(KeyValueFor("2009/10/06"), typeof(DateTime)).Should().BeTrue();

            stepArgumentTypeConverter.Setup(x => x.CanConvert("2009/10/06", It.IsAny<IBindingType>(), usCultureInfo)).Returns(false);
            Subject(anotherCultureContainer).CanRetrieve(KeyValueFor("2009/10/06"), typeof(DateTime)).Should().BeFalse();
        }

        private static KeyValuePair<string, string> KeyValueFor(string value)
        {
            // retrieving a value requires a key->value set, but this class
            // does not need the key... so we pass nothing for our tests
            return new System.Collections.Generic.KeyValuePair<string, string> ("", value);
        }

        private static StepTransformationValueRetriever Subject(IObjectContainer container)
        {
            return new StepTransformationValueRetriever(container.Resolve<IContextManager>(), container.Resolve<IStepArgumentTypeConverter>());
        }

        private static StepTransformationValueRetriever Subject()
        {
            return Subject(BuildAUseableContainerForTesting());
        }

        private static IObjectContainer BuildAUseableContainerForTesting()
        {
            // have to set up a container with a bunch of mocked stuff
            // in order to build a StepArgumentTypeConverter
            var container = new BoDi.ObjectContainer();

            var provider = new TechTalk.SpecFlow.Infrastructure.DefaultDependencyProvider();
            provider.RegisterTestRunnerDefaults(container);

            Mock<IBindingRegistry> bindingRegistryStub = new Mock<IBindingRegistry>();
            List<IStepArgumentTransformationBinding> stepTransformations = new List<IStepArgumentTransformationBinding>();
            bindingRegistryStub.Setup(br => br.GetStepTransformations()).Returns(stepTransformations);
            Mock<IBindingInvoker> methodBindingInvokerStub = new Mock<IBindingInvoker>();

            var stepArgumentTypeConverter = new StepArgumentTypeConverter(new Mock<ITestTracer>().Object, bindingRegistryStub.Object, new Mock<IContextManager>().Object, methodBindingInvokerStub.Object);

            // need a culture info as well
            RegisterBindingCulture(new CultureInfo("en-US"), container);

            // load them into the container
            container.RegisterInstanceAs<IStepArgumentTypeConverter>(stepArgumentTypeConverter);


            return container;
        }

        private static void RegisterBindingCulture(CultureInfo cultureInfo, IObjectContainer container)
        {
            var contextManagerMock = new Mock<IContextManager>();
            contextManagerMock.Setup(cm => cm.FeatureContext).Returns(new FeatureContext(null, cultureInfo));
            container.RegisterInstanceAs(contextManagerMock.Object);
        }
    }
}