using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using FluentAssertions;
using Moq;
using Xunit;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.RuntimeTests
{
    
    public class StepArgumentTypeConverterTests
    {
        private IStepArgumentTypeConverter _stepArgumentTypeConverter;
        private readonly Mock<IBindingInvoker> methodBindingInvokerStub = new Mock<IBindingInvoker>();
        private CultureInfo _enUSCulture;

        public StepArgumentTypeConverterTests()
        {
            Mock<IBindingRegistry> bindingRegistryStub = new Mock<IBindingRegistry>();
            List<IStepArgumentTransformationBinding> stepTransformations = new List<IStepArgumentTransformationBinding>();
            bindingRegistryStub.Setup(br => br.GetStepTransformations()).Returns(stepTransformations);

            _stepArgumentTypeConverter = new StepArgumentTypeConverter(new Mock<ITestTracer>().Object, bindingRegistryStub.Object, new Mock<IContextManager>().Object, methodBindingInvokerStub.Object);
            _enUSCulture = new CultureInfo("en-US", false);
        }

        [Fact]
        public void ShouldConvertStringToStringType()
        {
            var result = _stepArgumentTypeConverter.Convert("testValue", typeof(string), _enUSCulture);
            result.Should().Be("testValue");
        }

        [Fact]
        public void ShouldConvertStringToIntType()
        {
            var result = _stepArgumentTypeConverter.Convert("10", typeof(int), _enUSCulture);
            result.Should().Be(10);
        }

        [Fact]
        public void ShouldConvertStringToDateType()
        {
            var result = _stepArgumentTypeConverter.Convert("2009/10/06", typeof(DateTime), _enUSCulture);
            result.Should().Be(new DateTime(2009, 10, 06));
        }

        [Fact]
        public void ShouldConvertStringToFloatType()
        {
            var result = _stepArgumentTypeConverter.Convert("10.01", typeof(float), _enUSCulture);
            result.Should().Be(10.01f);
        }

        private enum TestEnumeration
        {
            Value1
        }

        [Fact]
        public void ShouldConvertStringToEnumerationType()
        {
            var result = _stepArgumentTypeConverter.Convert("Value1", typeof(TestEnumeration), _enUSCulture);
            result.Should().Be(TestEnumeration.Value1);
        }

        [Fact]
        public void ShouldConvertStringToEnumerationTypeWithDifferingCase()
        {
            var result = _stepArgumentTypeConverter.Convert("vALUE1", typeof(TestEnumeration), _enUSCulture);
            result.Should().Be(TestEnumeration.Value1);
        }

        [Fact]
        public void ShouldConvertStringToEnumerationTypeWithWhitespace()
        {
            var result = _stepArgumentTypeConverter.Convert("Value 1", typeof(TestEnumeration), _enUSCulture);
            result.Should().Be(TestEnumeration.Value1);
        }

        [Fact]
        public void ShouldConvertGuidToGuidType()
        {
            var result = _stepArgumentTypeConverter.Convert("{EF338B79-FD29-488F-8CA7-39C67C2B8874}", typeof (Guid), _enUSCulture);
            result.Should().Be(new Guid("{EF338B79-FD29-488F-8CA7-39C67C2B8874}"));           
        }

        [Fact]
        public void ShouldConvertNullableGuidToGuidType()
        {
            var result = _stepArgumentTypeConverter.Convert("{1081CFD1-F31F-420F-9360-40590ABEF887}", typeof(Guid?), _enUSCulture);
            result.Should().Be(new Guid("{1081CFD1-F31F-420F-9360-40590ABEF887}"));
        }

        [Fact]
        public void ShouldConvertNullableGuidWithEmptyValueToNull()
        {
            var result = _stepArgumentTypeConverter.Convert("", typeof(Guid?), _enUSCulture);
            result.Should().BeNull();
        }

        [Fact]
        public void ShouldConvertLooseGuids()
        {
            var result = _stepArgumentTypeConverter.Convert("1", typeof (Guid), _enUSCulture);
            result.Should().Be(new Guid("10000000-0000-0000-0000-000000000000"));
        }
        
        [Fact]
        public void ShouldUseATypeConverterWhenAvailable()
        {
            var originalValue = new DateTimeOffset(2019, 7, 29, 0, 0, 0, TimeSpan.Zero);
            var result = _stepArgumentTypeConverter.Convert(originalValue, typeof (TestClass), _enUSCulture);
            result.Should().BeOfType(typeof(TestClass));
            result.As<TestClass>().Time.Should().Be(originalValue);
        }

        [TypeConverter(typeof(TessClassTypeConverter))]
        class TestClass
        {
            public DateTimeOffset Time { get; set; }
            
            class TessClassTypeConverter : TypeConverter
            {
                public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
                {
                    return sourceType == typeof(DateTimeOffset);
                }

                public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
                {
                    return new TestClass { Time = (DateTimeOffset) value };
                }
            }
        }

   
    }
}