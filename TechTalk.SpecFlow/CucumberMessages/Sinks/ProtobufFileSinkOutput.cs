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
            switch (streamResult)
            {
                case IFailure<Stream> failure: return Result.Failure("Stream could not be opened", failure);
                case ISuccess<Stream> success: return WriteMessageToStream(success.Result, message);
                default: throw new InvalidOperationException($"The result from {nameof(BinaryFileAccessor.OpenAppendOrCreateFile)} must either implement {nameof(IFailure<Stream>)} or {nameof(ISuccess<Stream>)}");
            }
        }

        private IResult WriteMessageToStream(Stream target, IMessage message)
        {
            try
            {
                using (target)
                {
                    message.WriteTo(target);
                    target.Flush();
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
