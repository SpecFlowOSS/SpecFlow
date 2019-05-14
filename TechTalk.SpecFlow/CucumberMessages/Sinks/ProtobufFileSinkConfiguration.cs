namespace TechTalk.SpecFlow.CucumberMessages.Sinks
{
    public class ProtobufFileSinkConfiguration : IProtobufFileSinkConfiguration
    {
        public ProtobufFileSinkConfiguration(string targetFilePath)
        {
            TargetFilePath = targetFilePath;
        }

        public string TargetFilePath { get; }
    }
}
