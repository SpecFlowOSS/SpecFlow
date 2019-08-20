using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TechTalk.SpecFlow.Tracing;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests
{
    public class TraceListenerQueueTests
    {
        [Theory(DisplayName ="EnqueueMessage n times should yield messages synchronously")]
        [InlineData(2)]
        [InlineData(32)]
        [InlineData(128)]
        public void EnqueueMessage_nTimes_ShouldBeSynchronous(int times)
        {
            // ARRANGE
            var countdown = new CountdownEvent(times);
            var semaphore = new SemaphoreSlim(1, 1);
            var testOutputList = new List<string>();

            bool failureOnSemaphoreEntering = false;

            void WriteTestOutputCallback(string message)
            {
                countdown.Signal();
                if (!semaphore.Wait(0))
                {
                    failureOnSemaphoreEntering = true;
                    return;
                }

                testOutputList.Add(message);
                semaphore.Release();
            }

            var traceListenerMock = new Mock<ITraceListener>();
            traceListenerMock.Setup(l => l.WriteTestOutput(It.IsAny<string>()))
                             .Callback<string>(WriteTestOutputCallback);

            var testRunnerManagerMock = GetTestRunnerManagerMock();
            var testRunnerMock = GetTestRunnerMock();
            var traceListenerQueue = new TraceListenerQueue(traceListenerMock.Object, testRunnerManagerMock.Object);

            // ACT
            Parallel.For(0, times, i => traceListenerQueue.EnqueueMessage(testRunnerMock.Object, $"No. {i} - Thread {Thread.CurrentThread.ManagedThreadId}", false));

            // ASSERT
            countdown.Wait(TimeSpan.FromSeconds(10)).Should().BeTrue();
            failureOnSemaphoreEntering.Should().BeFalse();
            testOutputList.Should().HaveCount(times);
        }

        private Mock<ITestRunner> GetTestRunnerMock()
        {
            var testRunnerMock = new Mock<ITestRunner>();
            testRunnerMock.SetupGet(r => r.TestClassId)
                          .Returns(nameof(TraceListenerQueueTests));
            return testRunnerMock;
        }

        private Mock<ITestRunnerManager> GetTestRunnerManagerMock()
        {
            var testRunnerManagerMock = new Mock<ITestRunnerManager>();
            testRunnerManagerMock.SetupGet(m => m.IsMultiThreaded)
                                 .Returns(true);
            return testRunnerManagerMock;
        }
    }
}
