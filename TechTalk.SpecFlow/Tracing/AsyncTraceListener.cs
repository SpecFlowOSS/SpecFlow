using System;
using System.Collections.Generic;
using BoDi;

namespace TechTalk.SpecFlow.Tracing
{
    public class AsyncTraceListener : IBufferingTraceListener
    {
        private readonly Lazy<ITestRunner> _testRunner;
        private readonly ITraceListenerQueue _traceListenerQueue;
        private bool _isTraceBufferingActive;
        private List<TestTracerMessage> _bufferedMessages;

        public AsyncTraceListener(ITraceListenerQueue traceListenerQueue, IObjectContainer container)
        {
            _traceListenerQueue = traceListenerQueue;
            _testRunner = new Lazy<ITestRunner>(container.Resolve<ITestRunner>);
        }

        protected bool IsTraceBufferingActive => _isTraceBufferingActive;

        public virtual void WriteTestOutput(string message)
        {
            if (_isTraceBufferingActive)
            {
                _bufferedMessages.Add(new TestTracerMessage(message, false));
            }
            else
            {
                _traceListenerQueue.EnqueueMessage(_testRunner.Value, message, false);
            }
        }

        public virtual void WriteToolOutput(string message)
        {
            if (_isTraceBufferingActive)
            {
                _bufferedMessages.Add(new TestTracerMessage(message, true));
            }
            else
            {
                _traceListenerQueue.EnqueueMessage(_testRunner.Value, message, true);
            }
        }

        public void BufferingStart()
        {
            _bufferedMessages = new List<TestTracerMessage>();
            _isTraceBufferingActive = true;
        }

        public void BufferingFlushAndStop()
        {
            if (_isTraceBufferingActive)
            {
                // first remove flag, so that calls to WriteToolOutput and WriteTestOutput don't just re-buffer
                _isTraceBufferingActive = false;

                foreach (var bufferEntry in _bufferedMessages)
                {
                    // use WriteToolOutput and WriteTestOutput because they may be overwritten
                    if (bufferEntry.IsToolMessage)
                    {
                        WriteToolOutput(bufferEntry.Message);
                    }
                    else
                    {
                        WriteTestOutput(bufferEntry.Message);
                    }
                }

                _bufferedMessages = null;
            }
        }

        public void BufferingDiscardAndStop()
        {
            _isTraceBufferingActive = false;
            _bufferedMessages = null;
        }

        public struct TestTracerMessage
        {
            public TestTracerMessage(string message, bool isToolMessage)
            {
                Message = message;
                IsToolMessage = isToolMessage;
            }

            public string Message { get; }
            public bool IsToolMessage { get; }
        }
    }
}