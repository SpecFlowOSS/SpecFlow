using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using TechTalk.SpecFlow.Bindings;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.Bindings
{
    public class AsyncHelperTests
    {
        private void NormalMethod()
        {

        }

        private void ThrowException()
        {
            throw new Exception("Hi from async");
        }

        private Task AsyncMethodThrowsException()
        {
            throw new Exception();
        }

        [Fact]
        public void RunSync_NormalMethod()
        {
            AsyncHelpers.RunSync(() =>
            {
                NormalMethod();
                return Task.CompletedTask;
            });
        }

        [Fact]
        public void RunSync_ExceptionIsThrown()
        {

            Action action = () => AsyncHelpers.RunSync(() =>
            {
                ThrowException();
                return Task.CompletedTask;
            });

            action.Should().Throw<Exception>().WithMessage("Hi from async");
        }

        [Fact]
        public void RunSync_WhenExceptionIsThrown_ShouldRestoreOriginalStackTrace()
        {
            try
            {
                AsyncHelpers.RunSync(AsyncMethodThrowsException);
            }
            catch (Exception ex)
            {
                ex.StackTrace.TrimStart().Should().StartWith($"at {GetType().FullName}.{nameof(AsyncMethodThrowsException)}");
            }
        }

        [Fact]
        public void RunSync_WhenExceptionIsThrown_ShouldRestoreSynchronizationContext()
        {
            var expectedSynchronizationContext = SynchronizationContext.Current;

            Action action = () => AsyncHelpers.RunSync(AsyncMethodThrowsException);
            action.Should().Throw<Exception>();
      
            SynchronizationContext.Current.Should().BeSameAs(expectedSynchronizationContext);
        }

        private object NormalMethodReturnValue()
        {
            return null;
        }

        private object ThrowExceptionReturnValue()
        {
            throw new Exception("Hi from async");
        }

        private Task<object> AsyncMethodReturnValueThrowsException()
        {
            throw new Exception();
        }

        [Fact]
        public void RunSyncReturnValue_NormalMethod()
        {
            AsyncHelpers.RunSync(() => Task.FromResult(NormalMethodReturnValue()));
        }

        [Fact]
        public void RunSyncReturnValue_ExceptionIsThrown()
        {

            Action action = () => AsyncHelpers.RunSync(() =>
            {
                ThrowExceptionReturnValue();
                return Task.CompletedTask;
            });

            action.Should().Throw<Exception>().WithMessage("Hi from async");
        }

        [Fact]
        public void RunSyncReturnValue_WhenExceptionIsThrown_ShouldRestoreOriginalStackTrace()
        {
            try
            {
                AsyncHelpers.RunSync(AsyncMethodReturnValueThrowsException);
            }
            catch (Exception ex)
            {
                ex.StackTrace.TrimStart().Should().StartWith($"at {GetType().FullName}.{nameof(AsyncMethodReturnValueThrowsException)}");
            }
        }

        [Fact]
        public void RunSyncReturnValue_WhenExceptionIsThrown_ShouldRestoreSynchronizationContext()
        {
            var expectedSynchronizationContext = SynchronizationContext.Current;

            Action action = () => AsyncHelpers.RunSync(AsyncMethodReturnValueThrowsException);
            action.Should().Throw<Exception>();

            SynchronizationContext.Current.Should().BeSameAs(expectedSynchronizationContext);
        }
    }
}