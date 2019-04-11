using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using FluentAssertions;
using FluentAssertions.Common;
using TechTalk.SpecFlow.Assist.ValueRetrievers;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    public abstract class EnumerableRetrieverTests
    {
        protected const string FieldName = "Field";

        private static readonly CultureInfo TargetCulture = CultureInfo.GetCultureInfo("en-US");
        private static readonly DateTime[] TestDateTimes = { new DateTime(2011, 1, 1), new DateTime(2015, 12, 31), new DateTime(2011, 1, 1, 15, 16, 17) };
        private static readonly TimeSpan[] TestTimeSpans = { TimeSpan.Parse("20:00:00"), TimeSpan.Parse("00:00:00"), TimeSpan.Parse("6.12:14:45.3448") };
        private static readonly Guid[] TestGuids = Enumerable.Range(0, 3).Select(_ => Guid.NewGuid()).ToArray();

        public static IEnumerable<object[]> CanRetrieveData => RetrieveData
            .Select(x => new [] { x[0] })
            .ToArray();

        public static IEnumerable<object[]> RetrieveData =>
            new List<object[]>
            {
                new object[] { typeof(bool), new[] { "True", "False", "true" }, new [] { true, false, true } },
                new object[] { typeof(bool?), new[] { "True", "False", "" }, new bool?[] { true, false, null } },
                new object[] { typeof(byte), new[] { "0", "128", "255" }, new byte[] { 0, 128, 255 } },
                new object[] { typeof(byte?), new[] { "0", "128", "" }, new byte?[] { 0, 128, null } },
                new object[] { typeof(char), new[] { "a", "A", "1" }, new [] { 'a', 'A', '1' } },
                new object[] { typeof(char?), new[] { "a", "A", "" }, new char?[] { 'a', 'A', null } },
                new object[] { typeof(DateTime), TestDateTimes.Select(x => x.ToString(TargetCulture)).ToArray(), TestDateTimes },
                new object[] { typeof(DateTime?), new[] { TestDateTimes[0].ToString(TargetCulture), TestDateTimes[1].ToString(TargetCulture), "" }, new DateTime?[] { TestDateTimes[0], TestDateTimes[1], null } },
                new object[] { typeof(DateTimeOffset), TestDateTimes.Select(x => new DateTimeOffset(x).ToString(TargetCulture)).ToArray(), TestDateTimes.Select(x => new DateTimeOffset(x)).ToArray() },
                new object[] { typeof(DateTimeOffset?), new[] { TestDateTimes[0].ToString(TargetCulture), TestDateTimes[1].ToString(TargetCulture), "" }, new DateTimeOffset?[] { new DateTimeOffset(TestDateTimes[0]), new DateTimeOffset(TestDateTimes[1]), null } },
                new object[] { typeof(decimal), new[] { "0", "1.234", "-1.234" }, new [] { 0m, 1.234m, -1.234m } },
                new object[] { typeof(decimal?), new[] { "", "1.234", "-1.234" }, new decimal?[] { null, 1.234m, -1.234m } },
                new object[] { typeof(double), new[] { "0", "1.234", "-1.234" }, new [] { 0, 1.234, -1.234 } },
                new object[] { typeof(double?), new[] { "", "1.234", "-1.234" }, new double?[] { null, 1.234, -1.234 } },
                new object[] { typeof(TestEnum), new[] { nameof(TestEnum.Foo), nameof(TestEnum.Bar), nameof(TestEnum.FooBar) }, new object[] { TestEnum.Foo, TestEnum.Bar, TestEnum.FooBar } },
                new object[] { typeof(float), new[] { "0", "1.234", "-1.234" }, new [] { 0f, 1.234f, -1.234f } },
                new object[] { typeof(float?), new[] { "", "1.234", "-1.234" }, new float?[] { null, 1.234f, -1.234f } },
                new object[] { typeof(Guid), TestGuids.Select(x => x.ToString()), TestGuids },
                new object[] { typeof(Guid?), new[] { TestGuids[0].ToString(), TestGuids[1].ToString(), "" }, new Guid?[] { TestGuids[0], TestGuids[1], null } },
                new object[] { typeof(int), new[] { "0", "5", "-5" }, new [] { 0, 5, -5 } },
                new object[] { typeof(int?), new[] { "", "5", "-5" }, new int?[] { null, 5, -5 } },
                new object[] { typeof(long), new[] { "0", "5", "-5" }, new [] { 0L, 5L, -5L } },
                new object[] { typeof(long?), new[] { "", "5", "-5" }, new long?[] { null, 5L, -5L } },
                new object[] { typeof(sbyte), new[] { "0", "-100", "100" }, new sbyte[] { 0, -100, 100 } },
                new object[] { typeof(sbyte?), new[] { "", "-100", "100" }, new sbyte?[] { null, -100, 100 } },
                new object[] { typeof(short), new[] { "0", "1234", "-1234" }, new short[] { 0, 1234, -1234 } },
                new object[] { typeof(short?), new[] { "", "1234", "-1234" }, new short?[] { null, 1234, -1234 } },
                new object[] { typeof(string), new[] { "A", "B", "C" }, new object[] { "A", "B", "C" } },
                new object[] { typeof(TimeSpan), TestTimeSpans.Select(x => x.ToString()), TestTimeSpans },
                new object[] { typeof(TimeSpan?), new[] { TestTimeSpans[0].ToString(), TestTimeSpans[1].ToString(), "" }, new TimeSpan?[] { TestTimeSpans[0], TestTimeSpans[1], null } },
                new object[] { typeof(uint), new[] { "0", "1234", "5678" }, new uint[] { 0, 1234, 5678 } },
                new object[] { typeof(uint?), new[] { "0", "1234", "" }, new uint?[] { 0, 1234, null } },
                new object[] { typeof(ulong), new[] { "0", "1234", "5678" }, new ulong[] { 0, 1234, 5678 } },
                new object[] { typeof(ulong?), new[] { "0", "1234", "" }, new ulong?[] { 0, 1234, null } },
                new object[] { typeof(ushort), new[] { "0", "1234", "5678" }, new ushort[] { 0, 1234, 5678 } },
                new object[] { typeof(ushort?), new[] { "0", "1234", "" }, new ushort?[] { 0, 1234, null } },
            };

        protected EnumerableRetrieverTests()
        {
            Thread.CurrentThread.CurrentCulture = TargetCulture;
    }

        [Theory]
        [MemberData(nameof(CanRetrieveData))]
        public void Can_retrieve_value_enumeration_properties(Type valueType)
        {
            foreach (var propertyType in BuildPropertyTypes(valueType))
            {
                var retriever = CreateTestee();

                var actual = retriever.CanRetrieve(new KeyValuePair<string, string>(), null, propertyType);

                actual.Should().BeTrue();
            }
        }

        [Theory]
        [MemberData(nameof(RetrieveData))]
        public void Retrieves_value_enumeration_from_comma_separated_list(Type valueType, string[] textValues, IEnumerable expectedValues)
        {
            TestRetrieve(
                valueType,
                $"{textValues[0]},{textValues[1]},{textValues[2]}",
                expectedValues);
        }

        [Theory]
        [MemberData(nameof(RetrieveData))]
        public void Trims_the_individual_values(Type valueType, string[] textValues, IEnumerable expectedValues)
        {
            TestRetrieve(
                valueType,
                $"{textValues[0]} , {textValues[1]} , {textValues[2]}",
                expectedValues);
        }

        private void TestRetrieve(Type valueType, string fieldValue, IEnumerable expectedValues)
        {
            foreach (var propertyType in BuildPropertyTypes(valueType))
            {
                var retriever = CreateTestee();

                var resultObject = retriever.Retrieve(new KeyValuePair<string, string>(FieldName, fieldValue), null, propertyType);

                resultObject.Should().NotBeNull().And.BeAssignableTo(propertyType);
                ((IEnumerable)resultObject).Should().BeEquivalentTo(expectedValues, options => options.WithStrictOrdering());
            }
        }

        protected abstract EnumerableValueRetriever CreateTestee();
        
        protected abstract IEnumerable<Type> BuildPropertyTypes(Type valueType);
    }

    public enum TestEnum
    {
        Foo, Bar, FooBar
    }
}
