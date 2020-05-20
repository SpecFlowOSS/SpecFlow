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
            var expectedDefaultComparers = new object[]
            {
                typeof(DateTimeValueComparer),
                typeof(BoolValueComparer),
                typeof(GuidValueComparer),
                typeof(DecimalValueComparer),
                typeof(DoubleValueComparer),
                typeof(FloatValueComparer),
                typeof(DefaultValueComparer),
            };

            var service = new Service();

            var actualComparerTypes = service.ValueComparers.Select(x => x.GetType());

            actualComparerTypes.Should().BeEquivalentTo(expectedDefaultComparers);
        }

        [Fact]
        public void Should_load_the_value_retrievers_by_default()
        {
            var expectedDefaultRetrievers = new object[]
            {
                typeof(StringValueRetriever),
                typeof(ByteValueRetriever),
                typeof(SByteValueRetriever),
                typeof(IntValueRetriever),
                typeof(UIntValueRetriever),
                typeof(ShortValueRetriever),
                typeof(UShortValueRetriever),
                typeof(LongValueRetriever),
                typeof(ULongValueRetriever),
                typeof(FloatValueRetriever),
                typeof(DoubleValueRetriever),
                typeof(DecimalValueRetriever),
                typeof(CharValueRetriever),
                typeof(BoolValueRetriever),
                typeof(DateTimeValueRetriever),
                typeof(GuidValueRetriever),
                typeof(EnumValueRetriever),
                typeof(TimeSpanValueRetriever),
                typeof(DateTimeOffsetValueRetriever),
                typeof(UriValueRetriever),

                typeof(ArrayValueRetriever),
                typeof(ListValueRetriever),
            };

            var service = new Service();

            var actualRetrieverTypes = service.ValueRetrievers.Select(x => x.GetType());

            actualRetrieverTypes.Should().BeEquivalentTo(expectedDefaultRetrievers);
        }

        [Fact]
        public void Should_allow_the_removal_and_addition_of_new_value_comparers()
        {
            var service = new Service();

            foreach (var valueComparer in service.ValueComparers.ToArray())
            {
                service.ValueComparers.Unregister(valueComparer);
                Assert.DoesNotContain(valueComparer, service.ValueComparers);
            }

            var thing = new IExistsForTestingValueComparing();
            service.ValueComparers.Register(thing);
            Assert.Single(service.ValueComparers);
            Assert.Same(thing, service.ValueComparers.First());
        }

        [Fact]
        public void Should_allow_the_removal_and_addition_of_new_value_retrievers()
        {
            var service = new Service();

            foreach (var valueRetriever in service.ValueRetrievers.ToArray())
            {
                service.ValueRetrievers.Unregister(valueRetriever);
                Assert.DoesNotContain(valueRetriever, service.ValueRetrievers);
            }

            var thing = new IExistsForTestingValueRetrieving();
            service.ValueRetrievers.Register(thing);
            Assert.Single(service.ValueRetrievers);
            Assert.Same(thing, service.ValueRetrievers.First());
        }


        [Fact]
        public void Should_allow_for_the_restoration_of_the_defaults()
        {
            var service = new Service();

            foreach (var valueRetriever in service.ValueRetrievers.ToArray())
            {
                service.ValueRetrievers.Unregister(valueRetriever);
                Assert.DoesNotContain(valueRetriever, service.ValueRetrievers);
            }

            foreach (var valueComparer in service.ValueComparers.ToArray())
            {
                service.ValueComparers.Unregister(valueComparer);
                Assert.DoesNotContain(valueComparer, service.ValueComparers);
            }

            service.ValueRetrievers.Register(new IExistsForTestingValueRetrieving());
            service.ValueComparers.Register(new IExistsForTestingValueComparing());

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