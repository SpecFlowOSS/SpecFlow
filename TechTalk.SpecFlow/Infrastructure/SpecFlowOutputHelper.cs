using System;
using System.Collections.Generic;
using BoDi;
using TechTalk.SpecFlow.Events;

namespace TechTalk.SpecFlow.Infrastructure
{
    public class SpecFlowOutputHelper : ISpecFlowOutputHelper
    {
        private readonly IObjectContainer _container;
        private readonly ITestThreadExecutionEventPublisher _testThreadExecutionEventPublisher; 

        public SpecFlowOutputHelper(IObjectContainer container, ITestThreadExecutionEventPublisher testThreadExecutionEventPublisher)
        {
            _container = container;
            _testThreadExecutionEventPublisher = testThreadExecutionEventPublisher;
        }

        private IEnumerable<ISpecFlowScenarioOutputListener> Listeners =>
            _container.ResolveAll<ISpecFlowScenarioOutputListener>();

        public void WriteLine(string message)
        {
            _testThreadExecutionEventPublisher.PublishEvent(new OutputAddedEvent(message));
            foreach (var listener in Listeners)
            {
                listener.OnMessage(message);
            }
        }

        public void WriteLine(string format, params object[] args)
        {
            WriteLine(string.Format(format, args));
        }

        public void AddAttachment(string filePath)
        {
            _testThreadExecutionEventPublisher.PublishEvent(new AttachmentAddedEvent(filePath));
            foreach (var listener in Listeners)
            {
                listener.OnAttachmentAdded(filePath);
            }
        }
    }
}
