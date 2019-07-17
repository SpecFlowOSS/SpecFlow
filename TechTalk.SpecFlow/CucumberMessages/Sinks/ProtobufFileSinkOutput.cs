using System;
using System.IO;
using Google.Protobuf;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;
using TechTalk.SpecFlow.FileAccess;

namespace TechTalk.SpecFlow.CucumberMessages.Sinks
{
    public class ProtobufFileSinkOutput : IProtobufFileSinkOutput
    {
        private readonly IBinaryFileAccessor _binaryFileAccessor;
        private readonly ProtobufFileSinkConfiguration _protobufFileSinkConfiguration;
        private readonly IProtobufFileNameResolver _protobufFileNameResolver;

        public ProtobufFileSinkOutput(IBinaryFileAccessor binaryFileAccessor, ProtobufFileSinkConfiguration protobufFileSinkConfiguration, IProtobufFileNameResolver protobufFileNameResolver)
        {
            _binaryFileAccessor = binaryFileAccessor;
            _protobufFileSinkConfiguration = protobufFileSinkConfiguration;
            _protobufFileNameResolver = protobufFileNameResolver;
        }

        public IResult WriteMessage(Envelope message)
        {
            var resolveTargetFilePathResult = _protobufFileNameResolver.Resolve(_protobufFileSinkConfiguration.TargetFilePath);
            if (!(resolveTargetFilePathResult is ISuccess<string> resolveTargetFilePathSuccess))
            {
                switch (resolveTargetFilePathResult)
                {
                    case IFailure innerFailure: return Result.Failure("Stream could not be opened.", innerFailure);
                    default: return Result.Failure($"Stream could not be opened. File name '{_protobufFileSinkConfiguration.TargetFilePath}' could not be resolved.");
                }
            }

            var streamResult = _binaryFileAccessor.OpenAppendOrCreateFile(resolveTargetFilePathSuccess.Result);
            switch (streamResult)
            {
                case IFailure<Stream> failure: return Result.Failure("Stream could not be opened", failure);
                case ISuccess<Stream> success: return WriteMessageToStream(success.Result, message);
                default: throw new InvalidOperationException($"The result from {nameof(BinaryFileAccessor.OpenAppendOrCreateFile)} must either implement {nameof(IFailure<Stream>)} or {nameof(ISuccess<Stream>)}");
            }
        }

        private IResult WriteMessageToStream(Stream target, Envelope message)
        {
            try
            {
                using (target)
                {
                    message.WriteDelimitedTo(target);
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
