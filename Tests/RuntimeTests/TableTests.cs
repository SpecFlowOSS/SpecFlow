using System;
using NUnit.Framework;
namespace TechTalk.SpecFlow.RuntimeTests
{
    [TestFixture]
    public class TableTests
    {
        [Test]
        public void should_return_nice_error_message_when_column_not_is_found_in_table()
        {
            // Arrange
            var t = CreateDemoTable();

            // Act
            var mess = string.Empty;
            try
            {
                var h = t.Rows[0]["DontExsist"];
            }
            catch (IndexOutOfRangeException ex)
            {
                mess = ex.Message;
                Console.WriteLine(mess);
            }

            // Assert
            var expected = string.Format(Table.ERROR_COLUMN_NAME_NOT_FOUND, "DontExsist");
            Assert.IsTrue(mess.StartsWith(expected));
        }

        [Test]
        public void should_return_nice_errormessage_when_adding_null_as_headers()
        {
            // Act
            var mess = string.Empty;
            try
            {
                new Table(null);
            }
            catch (ArgumentException ex)
            {
                mess = ex.Message;
                Console.WriteLine(mess);
            }

            // Assert
            Assert.IsTrue(mess.StartsWith(Table.ERROR_NO_HEADER_TO_ADD));
        }

        [Test]
        public void should_return_nice_errormessage_when_adding_0_headers()
        {
            // Act
            var mess = string.Empty;
            try
            {
                var headers = new string[] { };
                new Table(headers);
            }
            catch (ArgumentException ex)
            {
                mess = ex.Message;
                Console.WriteLine(mess);
            }

            // Assert
            Assert.IsTrue(mess.StartsWith(Table.ERROR_NO_HEADER_TO_ADD));

        }

        [Test]
        public void should_return_nice_errormessage_when_adding_null_as_new_tablerow()
        {
            // Arrange
            var t = CreateDemoTable();

            // Act
            var mess = string.Empty;
            try
            {
                t.AddRow(null);
            }
            catch (Exception ex)
            {
                mess = ex.Message;
                Console.WriteLine(mess);
            }

            // Assert
            Assert.IsTrue(mess.StartsWith(Table.ERROR_NO_CELLS_TO_ADD));

        }

        [Test]
        public void should_return_nice_errormessage_when_cells_that_doesnt_match_headers()
        {
            // Arrange
            var t = CreateDemoTable();

            // Act
            var mess = string.Empty;
            try
            {
                t.AddRow("Only", "Two");
            }
            catch (ArgumentException ex)
            {
                mess = ex.Message;
                Console.WriteLine(mess);
            }

            // Assert
            var expected = string.Format(Table.ERROR_CELLS_NOT_MATCHING_HEADERS, 2, 3);
            Assert.IsTrue(mess.StartsWith(expected));
        }



        private static Table CreateDemoTable()
        {
            var t = new Table("Column 1", "Column 2", "Column 3");
            t.AddRow("Value 1:1", "Value 1:2", "Value 1:3");
            t.AddRow("Value 2:1", "Value 2:2", "Value 2:3");
            t.AddRow("Value 3:1", "Value 3:2", "Value 3:3");

            return t;
        }
    }
}