namespace TechTalk.SpecFlow.Configuration;

public class SpecFlowConfigFileNameProvider
{
    private static string DefaultSpecFlowJsonFileName = "specflow.json";

    public SpecFlowConfigFileNameProvider() : this(DefaultSpecFlowJsonFileName) { }

    public SpecFlowConfigFileNameProvider(string configFileName)
    {
        ConfigFileName = configFileName.IsNullOrEmpty() ? DefaultSpecFlowJsonFileName : configFileName;
    }

    public string ConfigFileName { get; }
}
