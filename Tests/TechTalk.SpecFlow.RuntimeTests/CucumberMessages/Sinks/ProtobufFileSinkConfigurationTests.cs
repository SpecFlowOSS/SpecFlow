using System;
using FluentAssertions;
using TechTalk.SpecFlow.CucumberMessages.Sinks;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.CucumberMessages.Sinks
{
    public class ProtobufFileSinkConfigurationTests
    {
        [Fact(DisplayName = @"Constructor should work with environment variables in the file path")]
        public void Constructor_ShouldWorkWithEnvironmentVariablesInTheFilePath()
        {
            // ARRANGE
            const string inputString = "%temp%/CucumberMessageQueue";
            string expectedFilePath = $"{Environment.GetEnvironmentVariable("temp")}/CucumberMessageQueue";

            // ACT
            var protobufFileSinkConfiguration = new ProtobufFileSinkConfiguration(inputString);

            // ASSERT
            protobufFileSinkConfiguration.TargetFilePath.Should().Be(expectedFilePath);
        }
    }
}
