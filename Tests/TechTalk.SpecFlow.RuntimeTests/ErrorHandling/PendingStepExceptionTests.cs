using System;
using FluentAssertions;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.ErrorHandling
{
    public class PendingStepExceptionTests
    {
        [Fact]
        public void Constructor_Message_AlwaysSet()
        {
            var pendingStepException = new PendingStepException();
            pendingStepException.Message.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void Constructor_Message_SetToValueFromConstructor()
        {
            var pendingStepException = new PendingStepException("custom pending message");
            pendingStepException.Message.Should().Be("custom pending message");
        }
    }
}
