using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using TechTalk.SpecFlow.Assist;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.TableHelperExtensionMethods
{
#if NET5_0
    public class RecordSupport
    {
        public record RecordType(string Property);

        [Fact]
        public void Works_With_Records()
        {
            var table = new Table("Property");
            table.AddRow("Row1");

            var record = table.CreateSet<RecordType>();
            record.Count().Should().Be(1);
        }


    }
#endif
}
