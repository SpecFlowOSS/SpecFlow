using System;

namespace TechTalk.SpecFlow.CucumberMessages.Sinks
{
    public class ProtobufFileSinkConfiguration
    {
        public ProtobufFileSinkConfiguration(string targetFilePath)
        {
            TargetFilePath = Environment.ExpandEnvironmentVariables(targetFilePath);
        }

        public string TargetFilePath { get; }
    }
}
