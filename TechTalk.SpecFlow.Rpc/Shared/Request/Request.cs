namespace TechTalk.SpecFlow.Rpc.Shared.Request
{
    public class Request : BaseRequest
    {
        public string Type { get; set; }
        public string Method { get; set; }
        public string Arguments { get; set; }
    }
}
