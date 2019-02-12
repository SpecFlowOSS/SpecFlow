using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests
{
    public class AsyncHelpersTests
    {
        [Fact]
        public void RunSync_WhenExceptionIsThrown_ShouldRestoreOriginalStackTrace()
        {
            try
            {
                AsyncHelpers.RunSync(AsyncMethodThrowsException);
            }
            catch (Exception ex)
            {
                ex.StackTrace.Should().StartWith("   at TechTalk.SpecFlow.RuntimeTests.AsyncHelpersTests.AsyncMethodThrowsException()");
            }
        }

        private Task AsyncMethodThrowsException()
        {
            throw new Exception();
        }
    }
}