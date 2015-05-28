using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.Tracing
{
    public interface ITraceListenerQueue : IDisposable
    {
        void Start();
        void EnqueueMessgage(ITestRunner sourceTestRunner, string message, bool isToolMessgae);
    }

    public class TraceListenerQueue : ITraceListenerQueue
    {
        struct TraceMessage
        {
            public bool IsToolMessage { get; set; }
            public string Message { get; set; }

            public TraceMessage(bool isToolMessage, string message) : this()
            {
                IsToolMessage = isToolMessage;
                Message = message;
            }
        }

        private readonly ITraceListener traceListener;
        private readonly ITestRunnerManager testRunnerManager;
        private readonly BlockingCollection<TraceMessage> messages = new BlockingCollection<TraceMessage>();
        private Task consumerTask;
        private Exception error = null;

        public TraceListenerQueue(ITraceListener traceListener, ITestRunnerManager testRunnerManager)
        {
            this.traceListener = traceListener;
            this.testRunnerManager = testRunnerManager;
        }

        public void Start()
        {
            consumerTask = Task.Factory.StartNew(() =>
            {
                try
                {
                    while (true)
                    {
                        var message = messages.Take();
                        ForwardMessage(message);
                    }
                }
                catch (InvalidOperationException)
                {
                }
                catch (Exception ex)
                {
                    this.error = ex;
                }
            });
        }

        private void ForwardMessage(TraceMessage message)
        {
            if (message.IsToolMessage)
                traceListener.WriteToolOutput(message.Message);
            else
                traceListener.WriteTestOutput(message.Message);
        }

        public void EnqueueMessgage(ITestRunner sourceTestRunner, string message, bool isToolMessgae)
        {
            if (error != null)
                throw new SpecFlowException("Trace listener failed.", error);

            if (!testRunnerManager.IsMultiThreaded)
            {
                // log synchronously
                ForwardMessage(new TraceMessage(isToolMessgae, message));
                return;
            }

            if (consumerTask == null)
            {
                lock (this)
                {
                    if (consumerTask == null)
                        Start();
                }
            }

            messages.Add(new TraceMessage(isToolMessgae, string.Format("#{1}: {0}", message, sourceTestRunner.ThreadId)));
        }

        public void Dispose()
        {
            if (consumerTask != null)
            {
                messages.CompleteAdding();
                consumerTask.Wait();
                consumerTask = null;
            }
        }
    }
}
