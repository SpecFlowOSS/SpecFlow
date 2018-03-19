namespace TechTalk.SpecFlow.Rpc.Shared.Request
{
    public class Request
    {
        public string Type { get; set; }
        public string Method { get; set; }
        public string Arguments { get; set; }
        public bool IsShutDown { get; set; }
    }
}
