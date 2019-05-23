using System;
using System.IO;
using Google.Protobuf;
using TechTalk.SpecFlow.CommonModels;
using TechTalk.SpecFlow.FileAccess;

namespace TechTalk.SpecFlow.CucumberMessages.Sinks
{
    public class ProtobufFileSinkOutput : IProtobufFileSinkOutput
    {
        private readonly IBinaryFileAccessor _binaryFileAccessor;
        private readonly ProtobufFileSinkConfiguration _protobufFileSinkConfiguration;

        public ProtobufFileSinkOutput(IBinaryFileAccessor binaryFileAccessor, ProtobufFileSinkConfiguration protobufFileSinkConfiguration)
        {
            _binaryFileAccessor = binaryFileAccessor;
            _protobufFileSinkConfiguration = protobufFileSinkConfiguration;
        }

        public IResult WriteMessage(IMessage message)
        {
            var streamResult = _binaryFileAccessor.OpenAppendOrCreateFile(_protobufFileSinkConfiguration.TargetFilePath);
            if (!(streamResult is ISuccess<Stream> success))
            {
                return Result.Failure("Stream could not be opened", streamResult);
            }

            try
            {
                using (success.Result)
                {
                    message.WriteTo(success.Result);
                    success.Result.Flush();
                    return Result.Success();
                }
            }
            catch (Exception e)
            {
                return Result.Failure(e);
            }
        }
    }
}
