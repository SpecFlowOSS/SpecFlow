namespace TechTalk.SpecFlow.CodeBehindGenerator.Shared.Request
{
    public class GenerateCodeBehindRequest : BaseRequest
    {
        public string SessionId { get; set; }
        public string FeatureFilePath { get; set; }
    }
}