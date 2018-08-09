using System;
using System.Linq;
using Xunit;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Assist.ValueComparers;
using TechTalk.SpecFlow.Assist.ValueRetrievers;
using System.Collections.Generic;
using FluentAssertions;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    
    public class ServiceTests
    {
        [Fact]
        public void Should_load_the_value_comparers_by_default()
        {
            var service = new Service();

            var results = service.ValueComparers;
            Assert.Equal(7, results.Count());

            Assert.Single(results.Where(x => x.GetType() == typeof(DateTimeValueComparer)));
            Assert.Single(results.Where(x => x.GetType() == typeof(BoolValueComparer)));
            Assert.Single(results.Where(x => x.GetType() == typeof(GuidValueComparer)));
            Assert.Single(results.Where(x => x.GetType() == typeof(DecimalValueComparer)));
            Assert.Single(results.Where(x => x.GetType() == typeof(DoubleValueComparer)));
            Assert.Single(results.Where(x => x.GetType() == typeof(FloatValueComparer)));
            Assert.Single(results.Where(x => x.GetType() == typeof(DefaultValueComparer)));
        }

        [Fact]
        public void Should_load_the_value_retrievers_by_default()
        {
            var service = new Service();

            var results = service.ValueRetrievers;

            results.Count().Should().Be(40);

            results.Count(x => x.GetType() == typeof(StringValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(ByteValueRetriever)).Should().Be(1); 
            results.Count(x => x.GetType() == typeof(SByteValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(IntValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(UIntValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(ShortValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(UShortValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(LongValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(ULongValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(FloatValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(DoubleValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(DecimalValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(CharValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(BoolValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(DateTimeValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(GuidValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(EnumValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(TimeSpanValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(DateTimeOffsetValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(NullableGuidValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(NullableDateTimeValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(NullableBoolValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(NullableCharValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(NullableDecimalValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(NullableDoubleValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(NullableFloatValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(NullableULongValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(NullableByteValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(NullableSByteValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(NullableIntValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(NullableUIntValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(NullableShortValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(NullableUShortValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(NullableLongValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(NullableTimeSpanValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(NullableDateTimeOffsetValueRetriever)).Should().Be(1);

            results.Count(x => x.GetType() == typeof(StringArrayValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(StringListValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(EnumArrayValueRetriever)).Should().Be(1);
            results.Count(x => x.GetType() == typeof(EnumListValueRetriever)).Should().Be(1);
        }

        [Fact]
        public void Should_allow_the_removal_and_addition_of_new_value_comparers()
        {
            var service = new Service();

            foreach (var valueComparer in service.ValueComparers.ToArray())
            {
                service.UnregisterValueComparer(valueComparer);
                Assert.DoesNotContain(valueComparer, service.ValueComparers);
            }

            var thing = new IExistsForTestingValueComparing();
            service.RegisterValueComparer(thing);
            Assert.Single(service.ValueComparers);
            Assert.Same(thing, service.ValueComparers.First());
        }

        [Fact]
        public void Should_allow_the_removal_and_addition_of_new_value_retrievers()
        {
            var service = new Service();

            foreach (var valueRetriever in service.ValueRetrievers.ToArray())
            {
                service.UnregisterValueRetriever(valueRetriever);
                Assert.DoesNotContain(valueRetriever, service.ValueRetrievers);
            }

            var thing = new IExistsForTestingValueRetrieving();
            service.RegisterValueRetriever(thing);
            Assert.Single(service.ValueRetrievers);
            Assert.Same(thing, service.ValueRetrievers.First());
        }


        [Fact]
        public void Should_allow_for_the_restoration_of_the_defaults()
        {
            var service = new Service();

            foreach (var valueRetriever in service.ValueRetrievers.ToArray())
            {
                service.UnregisterValueRetriever(valueRetriever);
                Assert.DoesNotContain(valueRetriever, service.ValueRetrievers);
            }

            foreach (var valueComparer in service.ValueComparers.ToArray())
            {
                service.UnregisterValueComparer(valueComparer);
                Assert.DoesNotContain(valueComparer, service.ValueComparers);
            }

            service.RegisterValueRetriever(new IExistsForTestingValueRetrieving());
            service.RegisterValueComparer(new IExistsForTestingValueComparing());

            service.RestoreDefaults();
        }

    }

    public class IExistsForTestingValueComparing : IValueComparer
    {
        public bool CanCompare(object actualValue)
        {
            throw new NotImplementedException();
        }
        public bool Compare(string expectedValue, object actualValue)
        {
            throw new NotImplementedException();
        }
    }

    public class IExistsForTestingValueRetrieving : IValueRetriever
    {
        public IEnumerable<Type> TypesForWhichIRetrieveValues()
        {
            throw new NotImplementedException();
        }
        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            throw new NotImplementedException();
        }
        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type type)
        {
            throw new NotImplementedException();
        }
    }
}