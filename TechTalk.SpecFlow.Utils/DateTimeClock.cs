using System;

namespace TechTalk.SpecFlow.Utils
{
    public class DateTimeClock : IClock
    {
        public DateTime GetToday()
        {
            return DateTime.Today;
        }

        public DateTime GetNowDateAndTime()
        {
            return DateTime.Now;
        }
    }
}
