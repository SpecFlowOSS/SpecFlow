using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Xml;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.Assist;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    namespace BasicTests
    {
        [TestFixture]
        public class BasicTests
        {
            [SetUp]
            public void TestSetup()
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            }

            public class UtilityTestA
            {
                public string Name { get; set; }
            }

            [Test]
            public void I_can_change_how_assist_works_by_changing_the_config()
            {
                var config = new Config();

                // a table with fake data
                var table = new Table("Field", "Value");
                table.AddRow("Name", "A name");

                // clear out the value retrievers
                foreach (var valueRetriever in config.ValueRetrievers.ToList())
                    config.UnregisterValueRetriever(valueRetriever);

                // load a fake value retriever that will return a value we expecte
                var expectedValue = Guid.NewGuid().ToString();

                var fakeValueRetriever = new Mock<IValueRetriever>();
                fakeValueRetriever.Setup(
                    x => x.CanRetrieve(It.IsAny<KeyValuePair<string, string>>(), It.IsAny<Type>(), It.IsAny<Type>()))
                    .Returns(true);
                fakeValueRetriever.Setup(
                    x => x.Retrieve(It.IsAny<KeyValuePair<string, string>>(), It.IsAny<Type>(), It.IsAny<Type>()))
                    .Returns(expectedValue);
                config.RegisterValueRetriever(fakeValueRetriever.Object);

                var tableService = new TableService(config);
                var result = tableService.CreateInstance<UtilityTestA>(table);

                // we expect the object that is built to have the expected name
                result.Name.ShouldBeEquivalentTo(expectedValue);
            }

            [Test]
            public void I_can_create_a_new_utility_class_with_a_new_config()
            {
                var config = new Config();
                new TableService(config);
            }

            [Test]
            public void Creating_a_new_service_sets_up_the_table_comparison_logic()
            {
                var service = new TableService(new Config());
                service.TableComparisonLogic.Should().NotBeNull();
                service.TableComparisonLogic.Service.Should().BeSameAs(service);
            }

            [Test]
            public void Creating_a_new_service_sets_up_the_table_creation_logic()
            {
                var service = new TableService(new Config());
                service.TableCreationLogic.Should().NotBeNull();
                service.TableCreationLogic.Service.Should().BeSameAs(service);
            }
        }

        [TestFixture]
        public class ComparisonTests
        {
            [SetUp]
            public void TestSetup()
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            }

            [Test]
            public void CompareToInstance_uses_the_table_comparison_logic()
            {
                var tableComparisonLogic = new Mock<ITableComparisonLogic>();
                var table = new Table("ignore");
                var instance = new Object();
                tableComparisonLogic.Setup(x => x.CompareToInstance(table, instance)).Verifiable();

                var service = new TableService(new Config());
                service.TableComparisonLogic = tableComparisonLogic.Object;

                service.CompareToInstance(table, instance);
                tableComparisonLogic.Verify();
            }

            [Test]
            public void CompareToSet_uses_the_table_comparison_logic()
            {
                var tableComparisonLogic = new Mock<ITableComparisonLogic>();
                var table = new Table("ignore");
                var set = new Object[] { new Object() };
                tableComparisonLogic.Setup(x => x.CompareToSet(table, set)).Verifiable();

                var service = new TableService(new Config());
                service.TableComparisonLogic = tableComparisonLogic.Object;

                service.CompareToSet(table, set);
                tableComparisonLogic.Verify();
            }
        }

        [TestFixture]
        public class CreationTests
        {
            [SetUp]
            public void TestSetup()
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            }

            [Test]
            public void CompareToInstance_uses_the_table_comparison_logic()
            {
                var service = new TableService(new Config());

                var table = new Table("Name");
                var name = Guid.NewGuid().ToString();
                table.AddRow(name);

                var result = service.CreateInstance<UtilityTestB>(table);

                result.Should().NotBeNull();
                result.Name.Should().BeEquivalentTo(name);
            }
        }

        public class UtilityTestB
        {
            public string Name { get; set; }
        }
    }
}