using System;
using System.Globalization;
using NUnit.Framework;

namespace TechTalk.SpecFlow.RuntimeTests
{
    [TestFixture]
    public class StepArgumentTypeConverterTests
    {
        private IStepArgumentTypeConverter _stepArgumentTypeConverter;
        private CultureInfo _enUSCulture;

        [SetUp]
        public void SetUp()
        {
            _stepArgumentTypeConverter = new StepArgumentTypeConverter();
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
        public void ShouldConvertStringToEnumerationTypeWithDifferingCase()
        {
            var result = _stepArgumentTypeConverter.Convert("vALUE1", typeof(TestEnumeration), _enUSCulture);
            Assert.That(result, Is.EqualTo(TestEnumeration.Value1));
        }
    }
}