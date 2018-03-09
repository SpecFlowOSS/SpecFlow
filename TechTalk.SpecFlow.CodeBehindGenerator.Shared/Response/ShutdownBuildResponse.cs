using System.IO;

namespace TechTalk.SpecFlow.CodeBehindGenerator.Shared.Response
{
    //public sealed class ShutdownBuildResponse : BuildResponse
    //{
    //    public readonly int ServerProcessId;

    //    public ShutdownBuildResponse(int serverProcessId)
    //    {
    //        ServerProcessId = serverProcessId;
    //    }

    //    public override ResponseType Type => ResponseType.Shutdown;

    //    protected override void AddResponseBody(BinaryWriter writer)
    //    {
    //        writer.Write(ServerProcessId);
    //    }

    //    public static ShutdownBuildResponse Create(BinaryReader reader)
    //    {
    //        var serverProcessId = reader.ReadInt32();
    //        return new ShutdownBuildResponse(serverProcessId);
    //    }
    //}
}