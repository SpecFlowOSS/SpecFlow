using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.CucumberMessages.Sinks;
using TechTalk.SpecFlow.FileAccess;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public interface ISinkProvider
    {
        List<ICucumberMessageSink> GetMessageSinksFromConfiguration();
    }

    public class SinkProvider : ISinkProvider
    {
        private readonly SpecFlowConfiguration _specFlowConfiguration;
        private readonly IBinaryFileAccessor _binaryFileAccessor;
        private readonly IProtobufFileNameResolver _protobufFileNameResolver;

        public SinkProvider(SpecFlowConfiguration specFlowConfiguration, IBinaryFileAccessor binaryFileAccessor, IProtobufFileNameResolver protobufFileNameResolver)
        {
            _specFlowConfiguration = specFlowConfiguration;
            _binaryFileAccessor = binaryFileAccessor;
            _protobufFileNameResolver = protobufFileNameResolver;
        }

        public List<ICucumberMessageSink> GetMessageSinksFromConfiguration()
        {
            if (_specFlowConfiguration?.CucumberMessagesConfiguration?.Enabled == false)
            {
                return new List<ICucumberMessageSink>();
            }

            if (_specFlowConfiguration?.CucumberMessagesConfiguration?.Sinks?.Any() == false)
            {
                var protobufFileSinkConfiguration = new ProtobufFileSinkConfiguration("CucumberMessageQueue/messages");
                return new List<ICucumberMessageSink>() {new ProtobufFileSink(new ProtobufFileSinkOutput(_binaryFileAccessor, protobufFileSinkConfiguration, _protobufFileNameResolver), protobufFileSinkConfiguration)};
            }


            var messageSinksFromConfiguration = new List<ICucumberMessageSink>();
            foreach (var cucumberMessagesSink in _specFlowConfiguration.CucumberMessagesConfiguration.Sinks)
            {
                switch (cucumberMessagesSink.Type.ToLower())
                {
                    case "file":
                        var protobufFileSinkConfiguration = new ProtobufFileSinkConfiguration(cucumberMessagesSink.Path);
                        messageSinksFromConfiguration.Add(new ProtobufFileSink(new ProtobufFileSinkOutput(_binaryFileAccessor, protobufFileSinkConfiguration, _protobufFileNameResolver), protobufFileSinkConfiguration));
                        break;
                    default:
                        throw new NotImplementedException($"The sink type {cucumberMessagesSink.Type}");
                }
            }

            
            return messageSinksFromConfiguration;
        }
    }
}