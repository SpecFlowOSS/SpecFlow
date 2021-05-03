using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Events
{
    public class TestThreadExecutionEventPublisher : ITestThreadExecutionEventPublisher
    {
        private readonly List<IExecutionEventListener> _listeners = new();
        private readonly Dictionary<Type, List<Delegate>> _handlersDictionary = new();

        public void PublishEvent(IExecutionEvent executionEvent)
        {
            foreach (var listener in _listeners)
            {
                listener.OnEvent(executionEvent);
            }

            if (_handlersDictionary.TryGetValue(executionEvent.GetType(), out var handlers))
            {
                foreach (var handler in handlers)
                {
                    handler.DynamicInvoke(executionEvent);
                }
            }
        }

        public void AddListener(IExecutionEventListener listener)
        {
            _listeners.Add(listener);
        }

        public void AddHandler<TEvent>(Action<TEvent> handler) where TEvent : IExecutionEvent
        {
            if (!_handlersDictionary.TryGetValue(typeof(TEvent), out var handlers))
            {
                handlers = new List<Delegate> { handler };
                _handlersDictionary.Add(typeof(TEvent), handlers);
            }
            else
            {
                handlers.Add(handler);
            }
        }
    }
}
