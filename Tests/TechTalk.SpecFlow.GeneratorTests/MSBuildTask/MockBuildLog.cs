using System;
using Xunit.Abstractions;

namespace TechTalk.SpecFlow.GeneratorTests.MSBuildTask
{
    public class MockBuildLog
    {
        private readonly ITestOutputHelper _output;

        public MockBuildLog(ITestOutputHelper output)
        {
            _output = output;
        }

        public void AddWarning(string message)
        {
            _output.WriteLine($"Warning: {message}");
        }

        public void AddError(string message)
        {
            _output.WriteLine($"Error: {message}");
        }

        public void AddMessage(string message)
        {
            _output.WriteLine(message);
        }
    }
}
