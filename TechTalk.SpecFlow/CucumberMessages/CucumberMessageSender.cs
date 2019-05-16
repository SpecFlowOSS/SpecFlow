using TechTalk.SpecFlow.Time;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class CucumberMessageSender : ICucumberMessageSender
    {
        private readonly IClock _clock;
        private readonly ICucumberMessageFactory _cucumberMessageFactory;
        private readonly ICucumberMessageSink _cucumberMessageSink;
        private bool _isInitialized;

        public CucumberMessageSender(IClock clock, ICucumberMessageFactory cucumberMessageFactory, ICucumberMessageSink cucumberMessageSink)
        {
            _clock = clock;
            _cucumberMessageFactory = cucumberMessageFactory;
            _cucumberMessageSink = cucumberMessageSink;
        }

        public bool Initialize()
        {
            if (_isInitialized)
            {
                return true;
            }

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

            _cucumberMessageSink.SendMessage(testRunStartedMessage);
        }
    }
}
