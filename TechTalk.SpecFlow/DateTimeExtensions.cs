using System;

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


    
        
    }
}