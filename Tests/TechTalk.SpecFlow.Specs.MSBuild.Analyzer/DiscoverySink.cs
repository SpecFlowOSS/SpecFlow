using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace TechTalk.SpecFlow.Specs.MSBuild.Analyzer
{
    public class DiscoverySink : IMessageSink, IMessageSinkWithTypes, IDisposable
    {
        private readonly DiscoveryEventSink _discoverySink;

        private readonly TaskCompletionSource<bool> _taskCompletionSource = new TaskCompletionSource<bool>();

        public DiscoverySink()
        {
            _discoverySink = new DiscoveryEventSink();

            _discoverySink.DiscoveryCompleteMessageEvent += args => _taskCompletionSource.SetResult(true);
        }

        public Task Finished => _taskCompletionSource.Task;

        public event MessageHandler<ITestCaseDiscoveryMessage> TestCaseDiscoveryMessageEvent
        {
            add => _discoverySink.TestCaseDiscoveryMessageEvent += value;
            remove => _discoverySink.TestCaseDiscoveryMessageEvent -= value;
        }

        public void Dispose() => _discoverySink.Dispose();

        bool IMessageSink.OnMessage(IMessageSinkMessage message)
            => OnMessageWithTypes(message, MessageSinkAdapter.GetImplementedInterfaces(message));

        public bool OnMessageWithTypes(IMessageSinkMessage message, HashSet<string> messageTypes)
            => _discoverySink.OnMessageWithTypes(message, messageTypes);
    }
}
