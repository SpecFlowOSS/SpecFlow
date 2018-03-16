namespace TechTalk.SpecFlow.Rpc.Shared.Response
{
    public class Response : BaseResponse
    {
        public string Type { get; set; }
        public string Method { get; set; }
        public string Result { get; set; }
    }
}
