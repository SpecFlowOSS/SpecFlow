using System.IO;

namespace TechTalk.SpecFlow.CodeBehindGenerator.Server
{
    public sealed class RejectedBuildResponse : BuildResponse
    {
        public override ResponseType Type => ResponseType.Rejected;

        /// <summary>
        /// AnalyzerInconsistency has no body.
        /// </summary>
        /// <param name="writer"></param>
        protected override void AddResponseBody(BinaryWriter writer) { }
    }
}