namespace TechTalk.SpecFlow.Plugins
{
    public class PluginDescriptor
    {
        public string Name { get; private set; }
        public string Path { get; private set; }
        public PluginType Type { get; private set; }
        public string Parameters { get; private set; }

        public PluginDescriptor(string name, string path, PluginType type, string parameters)
        {
            Name = name;
            Path = path;
            Type = type;
            Parameters = parameters;
        }
    }
}