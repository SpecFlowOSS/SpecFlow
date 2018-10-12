using System;
using FluentAssertions;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.Bindings
{
    public class AsyncHelperTests
    {
        public void NormalMethod()
        {

        }

        public void ThrowException()
        {
            throw new Exception("Hi from async");
        }


        [Fact]
        public void RunSync_NormalMethod()
        {
            AsyncHelpers.RunSync(async () => NormalMethod());
        }

        [Fact]
        public void RunSync_ExceptionIsThrown()
        {

            Action action = () => AsyncHelpers.RunSync(async () => ThrowException());

            action.Should().Throw<Exception>().WithMessage("Hi from async");
        }
    }
}