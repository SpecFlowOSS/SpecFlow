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
    public class StepTransformationValueRetrieverExample
    {
        public DateTime TheDate { get; set; } 
        public string TheString { get; set; }
    }

    [TestFixture]
    public class StepTransformationValueRetrieverTests
    {

        [Test]
        public void Retrieve_will_return_the_value_from_the_step_argument_type_converter()
        {
            var dateTimeResult = Subject().Retrieve(new KeyValuePair<string, string> ("TheDate", "2009/10/06"), typeof(StepTransformationValueRetrieverExample), typeof(DateTime));
            dateTimeResult.Should().Be(new DateTime(2009, 10, 6));

            var stringResult = Subject().Retrieve(new KeyValuePair<string, string> ("TheString", "2009/10/06"), typeof(StepTransformationValueRetrieverExample), typeof(string));
            stringResult.Should().Be("2009/10/06");
        }

        [Test]
        public void Retrieve_will_use_the_current_culture_info()
        {
            var stepArgumentTypeConverter = new Mock<IStepArgumentTypeConverter>();
            var value = Guid.NewGuid().ToString();

            //one culture
            var frenchSubject = Subject();

            var frenchCultureInfo = new CultureInfo("fr-FR");
            frenchSubject.ContainerToUseForThePurposeOfTesting.RegisterInstanceAs<IStepArgumentTypeConverter>(stepArgumentTypeConverter.Object);
            RegisterBindingCulture(frenchCultureInfo, frenchSubject.ContainerToUseForThePurposeOfTesting);

            var french = new Object();
            stepArgumentTypeConverter.Setup(x => x.Convert(value, It.IsAny<IBindingType>(), frenchCultureInfo)).Returns(french);
            frenchSubject.Retrieve(KeyValueFor(value), typeof(StepTransformationValueRetrieverExample), typeof(DateTime)).Should().BeSameAs(french);

            //another culture
            var usSubject = Subject();

            var usCultureInfo = new CultureInfo("fr-FR");
            usSubject.ContainerToUseForThePurposeOfTesting.RegisterInstanceAs<IStepArgumentTypeConverter>(stepArgumentTypeConverter.Object);
            RegisterBindingCulture(usCultureInfo, usSubject.ContainerToUseForThePurposeOfTesting);

            var us = new Object();
            stepArgumentTypeConverter.Setup(x => x.Convert(value, It.IsAny<IBindingType>(), usCultureInfo)).Returns(us);
            usSubject.Retrieve(KeyValueFor(value), typeof(StepTransformationValueRetrieverExample), typeof(DateTime)).Should().BeSameAs(us);
        }

        [Test]
        public void CanRetrieve_will_return_true_if_the_value_can_be_retrieved_from_a_step_argument_transformation()
        {
            Subject().CanRetrieve(KeyValueFor("2009/10/06"), null, typeof(DateTime)).Should().BeTrue();
            Subject().CanRetrieve(KeyValueFor("not a date"), null, typeof(DateTime)).Should().BeFalse();
            Subject().CanRetrieve(KeyValueFor("not a date"), null, typeof(string)).Should().BeTrue();
        }

        [Test]
        public void CanRetrieve_will_return_false_if_the_step_argument_transformation_work_is_throwing()
        {
            var subject = Subject();

            // removing the container here will cause the class to use the scenario context,
            // which was not set, so... it will throw
            subject.ContainerToUseForThePurposeOfTesting = null;

            subject.CanRetrieve(KeyValueFor("2009/10/06"), null, typeof(DateTime)).Should().BeFalse();
            subject.CanRetrieve(KeyValueFor("not a date"), null, typeof(DateTime)).Should().BeFalse();
            subject.CanRetrieve(KeyValueFor("not a date"), null, typeof(string)).Should().BeFalse();
        }

        [Test]
        public void CanRetriever_will_use_the_current_culture_info()
        {
            var subject = Subject();

            //one culture
            var frenchCultureInfo = new CultureInfo("fr-FR");
            var stepArgumentTypeConverter = new Mock<IStepArgumentTypeConverter>();
            subject.ContainerToUseForThePurposeOfTesting.RegisterInstanceAs<IStepArgumentTypeConverter>(stepArgumentTypeConverter.Object);
            RegisterBindingCulture(frenchCultureInfo, subject.ContainerToUseForThePurposeOfTesting);

            stepArgumentTypeConverter.Setup(x => x.CanConvert("2009/10/06", It.IsAny<IBindingType>(), frenchCultureInfo)).Returns(true);
            subject.CanRetrieve(KeyValueFor("2009/10/06"), null, typeof(DateTime)).Should().BeTrue();

            stepArgumentTypeConverter.Setup(x => x.CanConvert("2009/10/06", It.IsAny<IBindingType>(), frenchCultureInfo)).Returns(false);
            subject.CanRetrieve(KeyValueFor("2009/10/06"), null, typeof(DateTime)).Should().BeFalse();

            //another culture
            var subject2 = Subject();
            var usCultureInfo = new CultureInfo("en-US");
            subject2.ContainerToUseForThePurposeOfTesting.RegisterInstanceAs<IStepArgumentTypeConverter>(stepArgumentTypeConverter.Object);
            RegisterBindingCulture(usCultureInfo, subject2.ContainerToUseForThePurposeOfTesting);

            stepArgumentTypeConverter.Setup(x => x.CanConvert("2009/10/06", It.IsAny<IBindingType>(), usCultureInfo)).Returns(true);
            subject2.CanRetrieve(KeyValueFor("2009/10/06"), null, typeof(DateTime)).Should().BeTrue();

            stepArgumentTypeConverter.Setup(x => x.CanConvert("2009/10/06", It.IsAny<IBindingType>(), usCultureInfo)).Returns(false);
            subject2.CanRetrieve(KeyValueFor("2009/10/06"), null, typeof(DateTime)).Should().BeFalse();
        }

        private static KeyValuePair<string, string> KeyValueFor(string value)
        {
            // retrieving a value requires a key->value set, but this class
            // does not need the key... so we pass nothing for our tests
            return new System.Collections.Generic.KeyValuePair<string, string> ("TheDate", value);
        }

        private static StepTransformationValueRetriever Subject()
        {
            var retriever = new StepTransformationValueRetriever();
            retriever.ContainerToUseForThePurposeOfTesting = BuildAUseableContainerForTesting();
            return retriever;
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