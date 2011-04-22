using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.TestInfrastructure;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    [TestFixture]
    public class ProjectionTests
    {
        private SetComparisonTestObject testInstance;
        private IEnumerable<SetComparisonTestObject> testCollection;
        private Guid testGuid1 = Guid.NewGuid();
        private Guid testGuid2 = Guid.NewGuid();

        [SetUp]
        public void SetUp()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            testInstance = new SetComparisonTestObject
                        {
                            DateTimeProperty = DateTime.Today,
                            GuidProperty = testGuid1,
                            IntProperty = 1,
                            StringProperty = "a"
                        };
            testCollection = new SetComparisonTestObject[]
                {
                    testInstance,
                    new SetComparisonTestObject
                        {
                            DateTimeProperty = DateTime.Today, 
                            GuidProperty = testGuid2, 
                            IntProperty = 2, 
                            StringProperty = "b"
                        },
                };
        }

        [Test]
        public void Table_with_all_columns_same_rows_and_order_should_be_sequence_equal_to_collection()
        {
            var table = CreateTableWithAllColumns();
            table.AddRow(DateTime.Today.ToString(), testGuid1.ToString(), 1.ToString(), "a");
            table.AddRow(DateTime.Today.ToString(), testGuid2.ToString(), 2.ToString(), "b");

            Assert.IsTrue(table.ToProjection<SetComparisonTestObject>().SequenceEqual(testCollection.ToProjection()));
        }

        [Test]
        public void Table_with_all_columns_same_rows_but_different_order_should_not_be_sequence_equal_to_collection()
        {
            var table = CreateTableWithAllColumns();
            table.AddRow(DateTime.Today.ToString(), testGuid2.ToString(), 2.ToString(), "b");
            table.AddRow(DateTime.Today.ToString(), testGuid1.ToString(), 1.ToString(), "a");

            Assert.IsFalse(table.ToProjection<SetComparisonTestObject>().SequenceEqual(testCollection.ToProjection()));
        }

        [Test]
        public void Intersection_of_matching_table_and_collection_should_have_the_size_of_the_table()
        {
            var table = CreateTableWithAllColumns();
            table.AddRow(DateTime.Today.ToString(), testGuid1.ToString(), 1.ToString(), "a");
            table.AddRow(DateTime.Today.ToString(), testGuid2.ToString(), 2.ToString(), "b");

            Assert.AreEqual(table.RowCount, table.ToProjection<SetComparisonTestObject>().Intersect(testCollection.ToProjection()).Count());
        }

        [Test]
        public void Table_with_extra_columns_should_not_match_collection()
        {
            var table = CreateTableWithAllColumns();
            table.AddRow(DateTime.Today.AddDays(100).ToString(), Guid.NewGuid().ToString(), 1.ToString(), "a");
            table.AddRow(DateTime.Today.AddDays(200).ToString(), Guid.NewGuid().ToString(), 2.ToString(), "b");

            var query = from x in testCollection
                        select new { x.IntProperty, x.StringProperty };

            Assert.AreEqual(table.RowCount, table.ToProjectionOfSet(query).Except(query.ToProjection()).Count());
        }

        [Test]
        public void Table_with_subset_of_columns_with_matching_values_should_match_collection()
        {
            var table = CreateTableWithSubsetOfColumns();
            table.AddRow(1.ToString(), "a");
            table.AddRow(2.ToString(), "b");

            var query = from x in testCollection
                        select new { x.GuidProperty, x.IntProperty, x.StringProperty };

            Assert.AreEqual(0, table.ToProjectionOfSet(query).Except(query.ToProjection()).Count());
        }

        [Test]
        public void Table_with_subset_of_columns_with_matching_values_and_order_should_be_sequence_equal_to_collection()
        {
            var table = CreateTableWithSubsetOfColumns();
            table.AddRow(1.ToString(), "a");
            table.AddRow(2.ToString(), "b");

            var query = from x in testCollection
                        select new { x.GuidProperty, x.IntProperty, x.StringProperty };

            Assert.IsTrue(table.ToProjectionOfSet(query).SequenceEqual(query.ToProjection()));
        }

        [Test]
        public void Table_with_all_columns_same_rows_and_order_should_be_sequence_equal_to_queryable_collection()
        {
            var table = CreateTableWithAllColumns();
            table.AddRow(DateTime.Today.ToString(), testGuid1.ToString(), 1.ToString(), "a");
            table.AddRow(DateTime.Today.ToString(), testGuid2.ToString(), 2.ToString(), "b");

            var query = testCollection.AsQueryable();

            Assert.IsTrue(table.ToProjectionOfSet(query).SequenceEqual(query.ToProjection()));
        }

        [Test]
        public void Table_with_all_columns_same_rows_and_order_should_be_sequence_equal_to_list()
        {
            var table = CreateTableWithAllColumns();
            table.AddRow(DateTime.Today.ToString(), testGuid1.ToString(), 1.ToString(), "a");
            table.AddRow(DateTime.Today.ToString(), testGuid2.ToString(), 2.ToString(), "b");

            var query = testCollection.ToList();

            Assert.IsTrue(table.ToProjectionOfSet(query).SequenceEqual(query.ToProjection()));
        }

        [Test]
        public void Table_with_single_element_and_all_columns_should_be_equal_to_matching_instance()
        {
            var table = CreateTableWithAllColumns();
            table.AddRow(DateTime.Today.ToString(), testGuid1.ToString(), 1.ToString(), "a");

            Assert.AreEqual(table.ToProjection<SetComparisonTestObject>(), testInstance);
        }

        [Test]
        public void Table_with_single_element_and_all_columns_should_not_be_equal_to_unmatching_instance()
        {
            var table = CreateTableWithAllColumns();
            table.AddRow(DateTime.Today.ToString(), Guid.NewGuid().ToString(), 1.ToString(), "b");

            Assert.AreNotEqual(table.ToProjection<SetComparisonTestObject>(), testInstance);
        }

        [Test]
        public void Table_with_subset_of_columns_should_be_equal_to_matching_instance()
        {
            var table = CreateTableWithSubsetOfColumns();
            table.AddRow(1.ToString(), "a");

            var instance = new { IntProperty = testInstance.IntProperty, StringProperty = testInstance.StringProperty };

            Assert.AreEqual(table.ToProjectionOfInstance(instance), instance);
        }

        [Test]
        public void Table_with_subset_of_columns_should_not_be_equal_to_unmatching_instance()
        {
            var table = CreateTableWithSubsetOfColumns();
            table.AddRow(1.ToString(), "b");

            var instance = new { IntProperty = testInstance.IntProperty, StringProperty = testInstance.StringProperty };

            Assert.AreNotEqual(table.ToProjectionOfInstance(instance), instance);
        }

        private Table CreateTableWithAllColumns()
        {
            return new Table("DateTimeProperty", "GuidProperty", "IntProperty", "StringProperty");
        }

        private Table CreateTableWithSubsetOfColumns()
        {
            return new Table("IntProperty", "StringProperty");
        }
    }
}
