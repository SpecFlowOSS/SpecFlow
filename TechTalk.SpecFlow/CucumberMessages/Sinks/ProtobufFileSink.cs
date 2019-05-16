using System.IO;
using System.Threading;
using Google.Protobuf;

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

        public void SendMessage(IMessage message)
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
                    _protobufFileSinkOutput.WriteMessage(message);
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
        }
    }
}
