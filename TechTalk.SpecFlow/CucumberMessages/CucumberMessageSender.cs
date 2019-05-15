using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.CommonModels;
using TechTalk.SpecFlow.CucumberMessages.Configuration;
using TechTalk.SpecFlow.CucumberMessages.Sinks;
using TechTalk.SpecFlow.Time;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class CucumberMessageSender : ICucumberMessageSender
    {
        private readonly IClock _clock;
        private readonly ICucumberMessageFactory _cucumberMessageFactory;
        private readonly ISinkFactory _sinkFactory;
        private readonly ICucumberMessageSenderConfiguration _cucumberMessageSenderConfiguration;
        private IReadOnlyCollection<ICucumberMessageSink> _sinks;
        private bool _isInitialized;

        public CucumberMessageSender(IClock clock, ICucumberMessageFactory cucumberMessageFactory, ISinkFactory sinkFactory, ICucumberMessageSenderConfiguration cucumberMessageSenderConfiguration)
        {
            _clock = clock;
            _cucumberMessageFactory = cucumberMessageFactory;
            _sinkFactory = sinkFactory;
            _cucumberMessageSenderConfiguration = cucumberMessageSenderConfiguration;
        }

        public bool Initialize()
        {
            if (_isInitialized)
            {
                return true;
            }

            _sinks = _cucumberMessageSenderConfiguration.Sinks
                                                        .Select(sinkConfigEntry => new SinkConfiguration(sinkConfigEntry))
                                                        .Select(sinkConfig => _sinkFactory.FromConfiguration(sinkConfig))
                                                        .OfType<Success<ICucumberMessageSink>>()
                                                        .Select(success => success.Result)
                                                        .ToList();

            _isInitialized = true;
            return true;
        }

        public void SendTestRunStarted()
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            var testRunStartedMessage = _cucumberMessageFactory.BuildTestRunStartedMessage(_clock.GetNowDateAndTime());
            foreach (var cucumberMessageSink in _sinks)
            {
                cucumberMessageSink.SendMessage(testRunStartedMessage);
            }
        }
    }
}
