using System;
using Io.Cucumber.Messages;

namespace TechTalk.SpecFlow.RuntimeTests.CucumberMessages
{
    public static class TimestampExtensions
    {
        private static readonly DateTime UnixEpochStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime ToDateTime(this Timestamp timestamp)
        {
            return UnixEpochStart.AddSeconds(timestamp.Seconds);
        }

        public static ulong ToNanoseconds(this Duration duration)
        {
            return (ulong)(duration.Seconds * 1_000_000_000 + duration.Nanos);
        }
    }
}