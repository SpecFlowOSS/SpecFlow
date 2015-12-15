using System;
using System.Linq;
using NUnit.Framework;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Assist.ValueComparers;
using TechTalk.SpecFlow.Assist.ValueRetrievers;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    [TestFixture]
    public class ServiceTests
    {
        [Test]
        public void Should_load_the_value_comparers_by_default()
        {
            var service = new Service();

            var results = service.ValueComparers;
            Assert.AreEqual(7, results.Count());

            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(DateTimeValueComparer)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(BoolValueComparer)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(GuidValueComparer)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(DecimalValueComparer)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(DoubleValueComparer)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(FloatValueComparer)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(DefaultValueComparer)).Count());
        }

        [Test]
        public void Should_load_the_value_retrievers_by_default()
        {
            var service = new Service();

            var results = service.ValueRetrievers;
            Assert.AreEqual(36, results.Count());

            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(StringValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(ByteValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(SByteValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(IntValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(UIntValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(ShortValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(UShortValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(LongValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(ULongValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(FloatValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(DoubleValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(DecimalValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(CharValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(BoolValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(DateTimeValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(GuidValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(EnumValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(TimeSpanValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(DateTimeOffsetValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(NullableGuidValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(NullableDateTimeValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(NullableBoolValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(NullableCharValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(NullableDecimalValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(NullableDoubleValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(NullableFloatValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(NullableULongValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(NullableByteValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(NullableSByteValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(NullableIntValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(NullableUIntValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(NullableShortValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(NullableUShortValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(NullableLongValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(NullableTimeSpanValueRetriever)).Count());
            Assert.AreEqual(1, results.Where(x => x.GetType() == typeof(NullableDateTimeOffsetValueRetriever)).Count());
        }

        [Test]
        public void Should_allow_the_removal_and_addition_of_new_value_comparers()
        {
            var service = new Service();

            foreach (var valueComparer in service.ValueComparers.ToArray())
            {
                service.UnregisterValueComparer(valueComparer);
                Assert.IsFalse(service.ValueComparers.Contains(valueComparer));
            }

            var thing = new IExistsForTestingValueComparing();
            service.RegisterValueComparer(thing);
            Assert.AreEqual(1, service.ValueComparers.Count());
            Assert.AreSame(thing, service.ValueComparers.First());
        }

        [Test]
        public void Should_allow_the_removal_and_addition_of_new_value_retrievers()
        {
            var service = new Service();

            foreach (var valueRetriever in service.ValueRetrievers.ToArray())
            {
                service.UnregisterValueRetriever(valueRetriever);
                Assert.IsFalse(service.ValueRetrievers.Contains(valueRetriever));
            }

            var thing = new IExistsForTestingValueRetrieving();
            service.RegisterValueRetriever(thing);
            Assert.AreEqual(1, service.ValueRetrievers.Count());
            Assert.AreSame(thing, service.ValueRetrievers.First());
        }


        [Test]
        public void Should_allow_for_the_restoration_of_the_defaults()
        {
            var service = new Service();

            foreach (var valueRetriever in service.ValueRetrievers.ToArray())
            {
                service.UnregisterValueRetriever(valueRetriever);
                Assert.IsFalse(service.ValueRetrievers.Contains(valueRetriever));
            }

            foreach (var valueComparer in service.ValueComparers.ToArray())
            {
                service.UnregisterValueComparer(valueComparer);
                Assert.IsFalse(service.ValueComparers.Contains(valueComparer));
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
        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type type)
        {
            throw new NotImplementedException();
        }
    }
}