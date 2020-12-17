using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.Exceptions
{
    public class PendingStepExceptionTests
    {
        [Fact]
        public void Constructor_Message_AlwaysSet()
        {
            var pendingStepException = new PendingStepException();
            pendingStepException.Message.Should().NotBeNullOrWhiteSpace();
        }
        
        
    }
}
