using System;
using Google.Protobuf;

namespace TechTalk.SpecFlow.CucumberMessages.Sinks
{
    public class ProtobufFileSink : ICucumberMessageSink
    {
        private readonly IProtobufFileSinkOutput _protobufFileSinkOutput;
        private readonly object _lock;

        public ProtobufFileSink(IProtobufFileSinkOutput protobufFileSinkOutput, object @lock)
        {
            _protobufFileSinkOutput = protobufFileSinkOutput;
            _lock = @lock;
        }

        public void SendMessage(IMessage message)
        {
            lock (_lock)
            {
                if (!_protobufFileSinkOutput.EnsureIsInitialized())
                {
                    throw new InvalidOperationException("Target file could not be opened to write");
                }

                _protobufFileSinkOutput.WriteMessage(message);
            }
        }
    }
}
