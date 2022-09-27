using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Assist.ValueRetrievers;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests;
using Xunit;


namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.SituationalTests.EnumberableValueRetrieverIntegrationTests
{
    public class Example
    {
        public IList<int?> MyExampleValues { get; set; }

    }

    public class ListOfNullablesWithCustomNullValueRetrieverTests
    {

        private const string Val = "1, <null>";
        private TableRow testRow = new TableRow(new Table("Value"), new string[] {"list", Val});

        [Fact]
        public void CanRetrieveAListThatContainsValuesThatShouldBeRetrievedByACustomValueRetriever()
        {
            Service.Instance.ValueRetrievers.Register(new NullValueRetriever("<null>"));

            var retriever_under_test = Service.Instance.GetValueRetrieverFor(testRow, typeof(object), typeof(IList<Nullable<int>>));
            retriever_under_test.Should().NotBeNull();

            var can = retriever_under_test.CanRetrieve(Val, typeof(IList<Nullable<int>>));

            can.Should().BeTrue();

            var returned_list = retriever_under_test.Retrieve(new KeyValuePair<string, string>("Value", Val), typeof(object), typeof(IList<Nullable<int>>));

            returned_list.Should().NotBeNull();

            var listOfNullableInts = returned_list as IList<Nullable<int>>;

            listOfNullableInts.Should().NotBeNull();

            listOfNullableInts.Count.Should().Be(2);

            listOfNullableInts[0].Should().Be(1);
            listOfNullableInts[1].Should().BeNull();

        }

        public static IEnumerable<object[]> EnumerableRetrieverWithNullableValueRetriever_TestCases() =>
            new List<object[]>
            {
                new object[] { "1, 2", new List<int?> {1, 2 } },
                new object[] { "1, <null>", new List<int?> { 1, null} },
                new object[] { "<null>, 2", new List<int?> { null, 2} }
            };

        [Theory]
        [MemberData(nameof(EnumerableRetrieverWithNullableValueRetriever_TestCases))]
        public void IntegrationOfEnumerableRetrieverWithNullableValueRetriever(string input, IEnumerable<int?> expectedValues)
        {
            Service.Instance.ValueRetrievers.Register(new NullValueRetriever("<null>"));
            var table = new Table("Field", "Value");
            table.AddRow("MyExampleValues", input);

            var expected = expectedValues.ToList();

            var test = table.CreateInstance<Example>();
            test.MyExampleValues.Should().BeEquivalentTo(expected);


        }
    }
}
