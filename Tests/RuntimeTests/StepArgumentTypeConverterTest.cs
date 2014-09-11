using System;
using System.Collections.Generic;
using System.Globalization;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.RuntimeTests
{
    [TestFixture]
    public class StepArgumentTypeConverterTests
    {
        private IStepArgumentTypeConverter _stepArgumentTypeConverter;
        private readonly Mock<IBindingInvoker> methodBindingInvokerStub = new Mock<IBindingInvoker>();
        private CultureInfo _enUSCulture;

        [SetUp]
        public void SetUp()
        {
            Mock<IBindingRegistry> bindingRegistryStub = new Mock<IBindingRegistry>();
            List<IStepArgumentTransformationBinding> stepTransformations = new List<IStepArgumentTransformationBinding>();
            bindingRegistryStub.Setup(br => br.GetStepTransformations()).Returns(stepTransformations);

            _stepArgumentTypeConverter = new StepArgumentTypeConverter(new Mock<ITestTracer>().Object, bindingRegistryStub.Object, new Mock<IContextManager>().Object, methodBindingInvokerStub.Object);
            _enUSCulture = new CultureInfo("en-US");
        }

        [Test]
        public void ShouldConvertStringToStringType()
        {
            var result = _stepArgumentTypeConverter.Convert("testValue", typeof(string), _enUSCulture);
            Assert.That(result, Is.EqualTo("testValue"));
        }

        [Test]
        public void ShouldConvertStringToIntType()
        {
            var result = _stepArgumentTypeConverter.Convert("10", typeof(int), _enUSCulture);
            Assert.That(result, Is.EqualTo(10));
        }

        [Test]
        public void ShouldConvertStringToDateType()
        {
            var result = _stepArgumentTypeConverter.Convert("2009/10/06", typeof(DateTime), _enUSCulture);
            Assert.That(result, Is.EqualTo(new DateTime(2009, 10, 06)));
        }

        [Test]
        public void ShouldConvertStringToFloatType()
        {
            var result = _stepArgumentTypeConverter.Convert("10.01", typeof(float), _enUSCulture);
            Assert.That(result, Is.EqualTo(10.01f));
        }

        private enum TestEnumeration
        {
            Value1
        }

        [Test]
        public void ShouldConvertStringToEnumerationType()
        {
            var result = _stepArgumentTypeConverter.Convert("Value1", typeof(TestEnumeration), _enUSCulture);
            Assert.That(result, Is.EqualTo(TestEnumeration.Value1));
        }

        [Test]
        public void ShouldConvertStringToEnumerationTypeWithSpaces()
        {
            var result = _stepArgumentTypeConverter.Convert("Value 1", typeof(TestEnumeration), _enUSCulture);
            Assert.That(result, Is.EqualTo(TestEnumeration.Value1));
        }

        [Test]
        public void ShouldConvertStringToEnumerationTypeWithDifferingCase()
        {
            var result = _stepArgumentTypeConverter.Convert("vALUE1", typeof(TestEnumeration), _enUSCulture);
            Assert.That(result, Is.EqualTo(TestEnumeration.Value1));
        }

        [Test]
        public void ShouldConvertGuidToGuidType()
        {
            var result = _stepArgumentTypeConverter.Convert("{EF338B79-FD29-488F-8CA7-39C67C2B8874}", typeof (Guid), _enUSCulture);
            Assert.That(result, Is.EqualTo(new Guid("{EF338B79-FD29-488F-8CA7-39C67C2B8874}")));
        }

        [Test]
        public void ShouldConvertNullableGuidToGuidType()
        {
            var result = _stepArgumentTypeConverter.Convert("{1081CFD1-F31F-420F-9360-40590ABEF887}", typeof(Guid?), _enUSCulture);
            Assert.That(result, Is.EqualTo(new Guid("{1081CFD1-F31F-420F-9360-40590ABEF887}")));
        }

        [Test]
        public void ShouldConvertNullableGuidWithEmptyValueToNull()
        {
            var result = _stepArgumentTypeConverter.Convert("", typeof(Guid?), _enUSCulture);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void ShouldConvertLooseGuids()
        {
            var result = _stepArgumentTypeConverter.Convert("1", typeof (Guid), _enUSCulture);
            Assert.That(result, Is.EqualTo(new Guid("10000000-0000-0000-0000-000000000000")));
        }
    }
}