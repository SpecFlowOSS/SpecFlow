using System;
using Google.Protobuf;

namespace TechTalk.SpecFlow.CucumberMessages.Sinks
{
    public interface IProtobufFileSinkOutput : IDisposable
    {
        bool EnsureIsInitialized();

        bool WriteMessage(IMessage message);
    }
}
