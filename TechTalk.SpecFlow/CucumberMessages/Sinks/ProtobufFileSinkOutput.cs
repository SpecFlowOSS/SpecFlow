using System;
using System.IO;
using Google.Protobuf;
using TechTalk.SpecFlow.CommonModels;
using TechTalk.SpecFlow.CucumberMessages.Configuration;
using TechTalk.SpecFlow.FileAccess;

namespace TechTalk.SpecFlow.CucumberMessages.Sinks
{
    public class ProtobufFileSinkOutput : IProtobufFileSinkOutput
    {
        private readonly ISinkConfiguration _sinkConfiguration;
        private readonly IBinaryFileAccessor _binaryFileAccessor;
        private readonly ProtobufFileSinkConfigurationFactory _configurationFactory;
        private bool _isDisposed;

        public ProtobufFileSinkOutput(ISinkConfiguration sinkConfiguration, IBinaryFileAccessor binaryFileAccessor, ProtobufFileSinkConfigurationFactory configurationFactory)
        {
            _sinkConfiguration = sinkConfiguration;
            _binaryFileAccessor = binaryFileAccessor;
            _configurationFactory = configurationFactory;
        }

        public Stream OutputStream { get; private set; }

        public bool IsInitialized() => OutputStream is Stream stream && stream.CanWrite;

        public bool EnsureIsInitialized()
        {
            if (IsInitialized())
            {
                return true;
            }

            var sinkConfigurationResult = _configurationFactory.FromSinkConfiguration(_sinkConfiguration);
            if (!(sinkConfigurationResult is Success<ProtobufFileSinkConfiguration> configurationSuccess))
            {
                return false;
            }

            var streamResult = _binaryFileAccessor.OpenAppendOrCreateFile(configurationSuccess.Result.TargetFilePath);

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
