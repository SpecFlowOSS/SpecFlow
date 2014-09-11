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
            testCollection = new[]
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

            var setProjection = testCollection.ToProjection();
            var tableProjection = table.ToProjection<SetComparisonTestObject>();
            Assert.IsTrue(tableProjection.SequenceEqual(setProjection));
        }

        [Test]
        public void Table_with_all_columns_same_rows_but_different_order_should_not_be_sequence_equal_to_collection()
        {
            var table = CreateTableWithAllColumns();
            table.AddRow(DateTime.Today.ToString(), testGuid2.ToString(), 2.ToString(), "b");
            table.AddRow(DateTime.Today.ToString(), testGuid1.ToString(), 1.ToString(), "a");

            var tableProjection = table.ToProjection<SetComparisonTestObject>();
            var setProjection = testCollection.ToProjection();
            Assert.IsFalse(tableProjection.SequenceEqual(setProjection));
        }

        [Test]
        public void Table_with_all_columns_same_rows_but_different_order_should_throw_comparison_exception_for_sequence_equal()
        {
            var table = CreateTableWithAllColumns();
            table.AddRow(DateTime.Today.ToString(), testGuid2.ToString(), 2.ToString(), "b");
            table.AddRow(DateTime.Today.ToString(), testGuid1.ToString(), 1.ToString(), "a");

            Assert.Throws<ComparisonException>(() => 
                table.CompareToProjectionOfSet(testCollection, 
                (x, y) => x.SequenceEqual(y)));
        }

        [Test]
        public void Intersection_of_matching_table_and_collection_should_have_the_size_of_the_table()
        {
            var table = CreateTableWithAllColumns();
            table.AddRow(DateTime.Today.ToString(), testGuid1.ToString(), 1.ToString(), "a");
            table.AddRow(DateTime.Today.ToString(), testGuid2.ToString(), 2.ToString(), "b");

            var tableProjection = table.ToProjection<SetComparisonTestObject>();
            var setProjection = testCollection.ToProjection();
            Assert.AreEqual(table.RowCount, tableProjection.Intersect(setProjection).Count());
        }

        [Test]
        public void Table_with_extra_columns_should_not_match_collection()
        {
            var table = CreateTableWithAllColumns();
            table.AddRow(DateTime.Today.AddDays(100).ToString(), Guid.NewGuid().ToString(), 1.ToString(), "a");
            table.AddRow(DateTime.Today.AddDays(200).ToString(), Guid.NewGuid().ToString(), 2.ToString(), "b");

            var query = from x in testCollection
                        select new { x.IntProperty, x.StringProperty };

            var tableProjection = table.ToProjectionOfSet(query);
            var queryProjection = query.ToProjection();
            Assert.AreEqual(table.RowCount, tableProjection.Except(queryProjection).Count());
        }

        [Test]
        public void Table_with_extra_columns_should_throw_comparison_exception_on_except()
        {
            var table = CreateTableWithAllColumns();
            table.AddRow(DateTime.Today.AddDays(100).ToString(), Guid.NewGuid().ToString(), 1.ToString(), "a");
            table.AddRow(DateTime.Today.AddDays(200).ToString(), Guid.NewGuid().ToString(), 2.ToString(), "b");

            var query = from x in testCollection
                        select new { x.IntProperty, x.StringProperty };

            Assert.Throws<ComparisonException>(() => 
                table.CompareToProjectionOfSet(query, 
                (x, y) => x.Except(y), "StringProperty"));
        }

        [Test]
        public void Table_with_subset_of_columns_with_matching_values_should_match_collection()
        {
            var table = CreateTableWithSubsetOfColumns();
            table.AddRow(1.ToString(), "a");
            table.AddRow(2.ToString(), "b");

            var query = from x in testCollection
                        select new { x.GuidProperty, x.IntProperty, x.StringProperty };

            var tableProjection = table.ToProjectionOfSet(query);
            var queryProjection = query.ToProjection();
            Assert.AreEqual(0, tableProjection.Except(queryProjection).Count());
        }

        [Test]
        public void Table_with_subset_of_columns_with_matching_values_and_order_should_be_sequence_equal_to_collection()
        {
            var table = CreateTableWithSubsetOfColumns();
            table.AddRow(1.ToString(), "a");
            table.AddRow(2.ToString(), "b");

            var query = from x in testCollection
                        select new { x.GuidProperty, x.IntProperty, x.StringProperty };

            var tableProjection = table.ToProjectionOfSet(query);
            var queryProjection = query.ToProjection();
            Assert.IsTrue(tableProjection.SequenceEqual(queryProjection));
        }

        [Test]
        public void Table_with_all_columns_same_rows_and_order_should_be_sequence_equal_to_queryable_collection()
        {
            var table = CreateTableWithAllColumns();
            table.AddRow(DateTime.Today.ToString(), testGuid1.ToString(), 1.ToString(), "a");
            table.AddRow(DateTime.Today.ToString(), testGuid2.ToString(), 2.ToString(), "b");

            var query = testCollection.AsQueryable();

            var tableProjection = table.ToProjectionOfSet(query);
            var queryProjection = query.ToProjection();
            Assert.IsTrue(tableProjection.SequenceEqual(queryProjection));
        }

        [Test]
        public void Table_with_all_columns_same_rows_and_order_should_be_sequence_equal_to_list()
        {
            var table = CreateTableWithAllColumns();
            table.AddRow(DateTime.Today.ToString(), testGuid1.ToString(), 1.ToString(), "a");
            table.AddRow(DateTime.Today.ToString(), testGuid2.ToString(), 2.ToString(), "b");

            var query = testCollection.ToList();

            var tableProjection = table.ToProjectionOfSet(query);
            var queryProjection = query.ToProjection();
            Assert.IsTrue(tableProjection.SequenceEqual(queryProjection));
        }

        [Test]
        public void Table_with_single_element_and_all_columns_should_be_equal_to_matching_instance()
        {
            var table = CreateTableWithAllColumns();
            table.AddRow(DateTime.Today.ToString(), testGuid1.ToString(), 1.ToString(), "a");

            var tableProjection = table.ToProjection<SetComparisonTestObject>();
            Assert.AreEqual(tableProjection, testInstance);
        }

        [Test]
        public void Table_with_single_element_and_all_columns_should_not_be_equal_to_unmatching_instance()
        {
            var table = CreateTableWithAllColumns();
            table.AddRow(DateTime.Today.ToString(), Guid.NewGuid().ToString(), 1.ToString(), "b");

            var tableProjection = table.ToProjection<SetComparisonTestObject>();
            Assert.AreNotEqual(tableProjection, testInstance);
        }

        [Test]
        public void Table_with_single_element_and_all_columns_should_throw_comparison_exception_for_unmatching_instance()
        {
            var table = CreateTableWithAllColumns();
            table.AddRow(DateTime.Today.ToString(), Guid.NewGuid().ToString(), 1.ToString(), "b");

            Assert.Throws<ComparisonException>(() => table.CompareToProjectionOfInstance(testInstance));
        }

        [Test]
        public void Table_with_subset_of_columns_should_be_equal_to_matching_instance()
        {
            var table = CreateTableWithSubsetOfColumns();
            table.AddRow(1.ToString(), "a");

            var instance = new { IntProperty = testInstance.IntProperty, StringProperty = testInstance.StringProperty };

            var tableProjection = table.ToProjectionOfInstance(instance);
            Assert.AreEqual(tableProjection, instance);
        }

        [Test]
        public void Table_with_subset_of_columns_should_not_be_equal_to_unmatching_instance()
        {
            var table = CreateTableWithSubsetOfColumns();
            table.AddRow(1.ToString(), "b");

            var instance = new { IntProperty = testInstance.IntProperty, StringProperty = testInstance.StringProperty };

            var tableProjection = table.ToProjectionOfInstance(instance);
            Assert.AreNotEqual(tableProjection, instance);
        }

        [Test]
        public void Table_with_subset_of_columns_should_throw_comparison_exception_for_unmatching_instance()
        {
            var table = CreateTableWithSubsetOfColumns();
            table.AddRow(1.ToString(), "b");

            var instance = new { IntProperty = testInstance.IntProperty, StringProperty = testInstance.StringProperty };

            Assert.Throws<ComparisonException>(() => table.CompareToProjectionOfInstance(instance));
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
