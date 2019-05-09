using System;

namespace TechTalk.SpecFlow.Time
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
