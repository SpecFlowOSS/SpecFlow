using System;
using System.Linq;
using Xunit;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Assist.ValueComparers;
using TechTalk.SpecFlow.Assist.ValueRetrievers;
using System.Collections.Generic;

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

            Assert.Equal(1, results.Where(x => x.GetType() == typeof(DateTimeValueComparer)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(BoolValueComparer)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(GuidValueComparer)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(DecimalValueComparer)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(DoubleValueComparer)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(FloatValueComparer)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(DefaultValueComparer)).Count());
        }

        [Fact]
        public void Should_load_the_value_retrievers_by_default()
        {
            var service = new Service();

            var results = service.ValueRetrievers;
            Assert.Equal(36, results.Count());

            Assert.Equal(1, results.Where(x => x.GetType() == typeof(StringValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(ByteValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(SByteValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(IntValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(UIntValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(ShortValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(UShortValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(LongValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(ULongValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(FloatValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(DoubleValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(DecimalValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(CharValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(BoolValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(DateTimeValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(GuidValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(EnumValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(TimeSpanValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(DateTimeOffsetValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(NullableGuidValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(NullableDateTimeValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(NullableBoolValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(NullableCharValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(NullableDecimalValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(NullableDoubleValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(NullableFloatValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(NullableULongValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(NullableByteValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(NullableSByteValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(NullableIntValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(NullableUIntValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(NullableShortValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(NullableUShortValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(NullableLongValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(NullableTimeSpanValueRetriever)).Count());
            Assert.Equal(1, results.Where(x => x.GetType() == typeof(NullableDateTimeOffsetValueRetriever)).Count());
        }

        [Fact]
        public void Should_allow_the_removal_and_addition_of_new_value_comparers()
        {
            var service = new Service();

            foreach (var valueComparer in service.ValueComparers.ToArray())
            {
                service.UnregisterValueComparer(valueComparer);
                Assert.False(service.ValueComparers.Contains(valueComparer));
            }

            var thing = new IExistsForTestingValueComparing();
            service.RegisterValueComparer(thing);
            Assert.Equal(1, service.ValueComparers.Count());
            Assert.Same(thing, service.ValueComparers.First());
        }

        [Fact]
        public void Should_allow_the_removal_and_addition_of_new_value_retrievers()
        {
            var service = new Service();

            foreach (var valueRetriever in service.ValueRetrievers.ToArray())
            {
                service.UnregisterValueRetriever(valueRetriever);
                Assert.False(service.ValueRetrievers.Contains(valueRetriever));
            }

            var thing = new IExistsForTestingValueRetrieving();
            service.RegisterValueRetriever(thing);
            Assert.Equal(1, service.ValueRetrievers.Count());
            Assert.Same(thing, service.ValueRetrievers.First());
        }


        [Fact]
        public void Should_allow_for_the_restoration_of_the_defaults()
        {
            var service = new Service();

            foreach (var valueRetriever in service.ValueRetrievers.ToArray())
            {
                service.UnregisterValueRetriever(valueRetriever);
                Assert.False(service.ValueRetrievers.Contains(valueRetriever));
            }

            foreach (var valueComparer in service.ValueComparers.ToArray())
            {
                service.UnregisterValueComparer(valueComparer);
                Assert.False(service.ValueComparers.Contains(valueComparer));
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