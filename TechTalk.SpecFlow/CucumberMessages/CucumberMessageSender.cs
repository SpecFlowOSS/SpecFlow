using System;
using TechTalk.SpecFlow.Time;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class CucumberMessageSender : ICucumberMessageSender
    {
        private readonly IClock _clock;

        public CucumberMessageSender(IClock clock)
        {
            _clock = clock;
        }

        public void SendTestRunStarted()
        {
        }
    }
}
