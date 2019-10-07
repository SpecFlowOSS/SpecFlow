using System;
using Io.Cucumber.Messages;

namespace TechTalk.SpecFlow
{
    internal static class DateTimeExtensions
    {
        private static DateTime UnitEpochStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

        public static TimeSpan DurationSinceUnixEpoch(this DateTime dateTime)
        {
            return dateTime.ToUniversalTime() - UnitEpochStart;
        }

        public static (long, int) ToUnixTimeStamp(this DateTime dateTime)
        {
            var durationSinceUnixEpoch = dateTime.DurationSinceUnixEpoch();
            var datetimeWithoutTicks = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Kind);

            var ticksDifference = dateTime.Ticks - datetimeWithoutTicks.Ticks;



            return ((long) durationSinceUnixEpoch.TotalSeconds, (int)ticksDifference * 100);
        }


        public static Timestamp ToCucumberMessagesTimestamp(this DateTime dateTime)
        {
            var (seconds, nanos) = dateTime.ToUnixTimeStamp();

            return new Timestamp()
            {
                Seconds = seconds,
                Nanos = nanos
            };
        }
        
        
        public static Duration ToCucumberMessagesDuration(this TimeSpan timeSpan)
        {
            var timeSpanWithoutTicks = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

            var ticksDifference = timeSpan.Ticks - timeSpanWithoutTicks.Ticks;

            return new Duration()
            {
                Seconds = (int)timeSpan.TotalSeconds,
                Nanos = (int)ticksDifference*100
            };
        }
        
    }
}