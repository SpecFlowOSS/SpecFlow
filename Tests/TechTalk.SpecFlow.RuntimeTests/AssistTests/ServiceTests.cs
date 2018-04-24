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
            Assert.Equal(36, results.Count());

            Assert.Single(results.Where(x => x.GetType() == typeof(StringValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(ByteValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(SByteValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(IntValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(UIntValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(ShortValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(UShortValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(LongValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(ULongValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(FloatValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(DoubleValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(DecimalValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(CharValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(BoolValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(DateTimeValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(GuidValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(EnumValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(TimeSpanValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(DateTimeOffsetValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(NullableGuidValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(NullableDateTimeValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(NullableBoolValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(NullableCharValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(NullableDecimalValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(NullableDoubleValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(NullableFloatValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(NullableULongValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(NullableByteValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(NullableSByteValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(NullableIntValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(NullableUIntValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(NullableShortValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(NullableUShortValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(NullableLongValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(NullableTimeSpanValueRetriever)));
            Assert.Single(results.Where(x => x.GetType() == typeof(NullableDateTimeOffsetValueRetriever)));
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