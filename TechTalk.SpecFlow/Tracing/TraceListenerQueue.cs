using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.Tracing
{
    public class TraceListenerQueue : ITraceListenerQueue
    {
        private readonly BlockingCollection<TraceMessage> _messages = new BlockingCollection<TraceMessage>();
        private readonly ITestRunnerManager _testRunnerManager;

        private readonly ITraceListener _traceListener;
        private readonly bool _isThreadSafeTraceListener;
        private Task _consumerTask;
        private Exception _error = null;

        public TraceListenerQueue(ITraceListener traceListener, ITestRunnerManager testRunnerManager)
        {
            _traceListener = traceListener;
            _testRunnerManager = testRunnerManager;
            _isThreadSafeTraceListener = traceListener is IThreadSafeTraceListener;
        }

        public void Start()
        {
            _consumerTask = Task.Factory.StartNew(() =>
            {
                try
                {
                    while (true)
                    {
                        var message = _messages.Take();
                        ForwardMessage(message);
                    }
                }
                catch (InvalidOperationException)
                {
                }
                catch (Exception ex)
                {
                    _error = ex;
                }
            });
        }

        public void EnqueueMessage(ITestRunner sourceTestRunner, string message, bool isToolMessgae)
        {
            if (_error != null)
                throw new SpecFlowException("Trace listener failed.", _error);

            if (_isThreadSafeTraceListener || !_testRunnerManager.IsMultiThreaded)
            {
                // log synchronously
                ForwardMessage(new TraceMessage(isToolMessgae, message));
                return;
            }

            if (_consumerTask == null)
            {
                lock (this)
                {
                    if (_consumerTask == null)
                        Start();
                }
            }

            _messages.Add(new TraceMessage(isToolMessgae, string.Format("#{1}: {0}", message, sourceTestRunner.TestClassId)));
        }

        public void Dispose()
        {
            if (_consumerTask != null)
            {
                _messages.CompleteAdding();
                _consumerTask.Wait();
                _consumerTask = null;
            }
        }

        private void ForwardMessage(TraceMessage message)
        {
            if (message.IsToolMessage)
                _traceListener.WriteToolOutput(message.Message);
            else
                _traceListener.WriteTestOutput(message.Message);
        }

        struct TraceMessage
        {
            public bool IsToolMessage { get; }
            public string Message { get; }

            public TraceMessage(bool isToolMessage, string message) : this()
            {
                IsToolMessage = isToolMessage;
                Message = message;
            }
        }
    }
}