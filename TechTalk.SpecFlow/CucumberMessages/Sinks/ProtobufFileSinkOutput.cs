using System;
using System.IO;
using Google.Protobuf;
using TechTalk.SpecFlow.CommonModels;
using TechTalk.SpecFlow.FileAccess;

namespace TechTalk.SpecFlow.CucumberMessages.Sinks
{
    public class ProtobufFileSinkOutput : IProtobufFileSinkOutput
    {
        private readonly IProtobufFileSinkConfiguration _protobufFileSinkConfiguration;
        private readonly IBinaryFileAccessor _binaryFileAccessor;
        private bool _isDisposed;

        public ProtobufFileSinkOutput(IProtobufFileSinkConfiguration protobufFileSinkConfiguration, IBinaryFileAccessor binaryFileAccessor)
        {
            _protobufFileSinkConfiguration = protobufFileSinkConfiguration;
            _binaryFileAccessor = binaryFileAccessor;
        }

        public Stream OutputStream { get; private set; }

        public bool IsInitialized() => OutputStream is Stream stream && stream.CanWrite;

        public bool EnsureIsInitialized()
        {
            if (IsInitialized())
            {
                return true;
            }

            var streamResult = _binaryFileAccessor.OpenAppendOrCreateFile(_protobufFileSinkConfiguration.TargetFilePath);

            if (!(streamResult is Success<Stream> success))
            {
                return false;
            }

            OutputStream = success.Result;
            return true;
        }

        public bool WriteMessage(IMessage message)
        {
            if (!IsInitialized())
            {
                return false;
            }

            try
            {
                message.WriteTo(OutputStream);
                OutputStream.Flush();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            OutputStream?.Dispose();
            _isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
