using System.IO;

namespace TechTalk.SpecFlow.CodeBehindGenerator.Server
{
    public sealed class MismatchedVersionBuildResponse : BuildResponse
    {
        public override ResponseType Type => ResponseType.MismatchedVersion;

        /// <summary>
        /// MismatchedVersion has no body.
        /// </summary>
        protected override void AddResponseBody(BinaryWriter writer) { }
    }
}