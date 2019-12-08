using System;
using FluentAssertions;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.Infrastructure
{
    public class DateTimeExtensionsTests
    {
        [Fact]
        public void UnitEpoch_ConvertedToUnitEpoch_Everything_is_Null()
        {
            var utcEpoch = new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc);

            var (seconds, nano) = utcEpoch.ToUnixTimeStamp();

            seconds.Should().Be(0);
            nano.Should().Be(0);
        }


        [Fact]
        public void FiveSecondsAfter_UnitEpoch_ConvertedToUnitEpoch_Seconds_Is_5()
        {
            var utcEpoch = new DateTime(1970, 1, 1, 0, 0, 5, DateTimeKind.Utc);

            var (seconds, nano) = utcEpoch.ToUnixTimeStamp();

            seconds.Should().Be(5);
            nano.Should().Be(0);
        }

        [Fact]
        public void FiveTicksAfter_UnitEpoch_ConvertedToUnitEpoch_NanoSeconds_Is_500()
        {
            var utcEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            utcEpoch = utcEpoch.AddTicks(5);

            var (seconds, nano) = utcEpoch.ToUnixTimeStamp();

            seconds.Should().Be(0);
            nano.Should().Be(500);
        }
    }
}