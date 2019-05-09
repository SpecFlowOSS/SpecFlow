using System.Collections.Generic;
using TechTalk.SpecFlow.Time;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class CucumberMessageSender : ICucumberMessageSender
    {
        private readonly IClock _clock;
        private readonly ICucumberMessageFactory _cucumberMessageFactory;
        private readonly IEnumerable<ICucumberMessageSink> _cucumberMessageSinks;

        public CucumberMessageSender(IClock clock, ICucumberMessageFactory cucumberMessageFactory, IEnumerable<ICucumberMessageSink> cucumberMessageSinks)
        {
            _clock = clock;
            _cucumberMessageFactory = cucumberMessageFactory;
            _cucumberMessageSinks = cucumberMessageSinks;
        }

        public void SendTestRunStarted()
        {
            var testRunStartedMessage = _cucumberMessageFactory.BuildTestRunStartedMessage(_clock.GetNowDateAndTime());
            foreach (var cucumberMessageSink in _cucumberMessageSinks)
            {
                cucumberMessageSink.SendMessage(testRunStartedMessage);
            }
        }
    }
}
