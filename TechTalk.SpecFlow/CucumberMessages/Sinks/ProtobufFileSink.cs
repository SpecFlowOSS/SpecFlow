using System;
using System.IO;
using System.Threading;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages.Sinks
{
    public class ProtobufFileSink : ICucumberMessageSink
    {
        private readonly IProtobufFileSinkOutput _protobufFileSinkOutput;
        private readonly ProtobufFileSinkConfiguration _protobufFileSinkConfiguration;

        public ProtobufFileSink(IProtobufFileSinkOutput protobufFileSinkOutput, ProtobufFileSinkConfiguration protobufFileSinkConfiguration)
        {
            _protobufFileSinkOutput = protobufFileSinkOutput;
            _protobufFileSinkConfiguration = protobufFileSinkConfiguration;
        }

        public void SendMessage(Envelope message)
        {
            string absoluteTargetFilePath = Path.GetFullPath(_protobufFileSinkConfiguration.TargetFilePath)
                                                .Replace('\\', '_')
                                                .Replace('/', '_')
                                                .Replace(':', '_');
            using (var mutex = new Mutex(false, $@"Global\SpecFlowTestExecution_{absoluteTargetFilePath}"))
            {
                mutex.WaitOne();

                try
                {
                    var result = _protobufFileSinkOutput.WriteMessage(message);
                    switch (result)
                    {
                        case ExceptionFailure exceptionFailure: throw exceptionFailure.Exception;
                        case Failure failure: throw new InvalidOperationException($"Could not write to file {_protobufFileSinkConfiguration.TargetFilePath}. {failure.Description}");
                        case IFailure _: throw new InvalidOperationException($"Could not write to file {_protobufFileSinkConfiguration.TargetFilePath}.");
                    }
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
        }
    }
}
