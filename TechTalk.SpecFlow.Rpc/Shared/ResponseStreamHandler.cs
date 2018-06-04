using System.IO;
using System.Text;
using Utf8Json;

namespace TechTalk.SpecFlow.Rpc.Shared
{
    public class ResponseStreamHandler
    {
        public static T Read<T>(Stream stream) where T : Response.Response
        {
            using (var reader = new BinaryReader(stream, Encoding.Default, true))
            {
                var length = reader.ReadInt32();
                var requestJson = new string(reader.ReadChars(length));

                return JsonSerializer.Deserialize<T>(requestJson, SerializationOptions.Current);
            }
        }

        public static void Write<T>(T request, Stream stream) where T : Response.Response
        {
            byte[] requestJson = JsonSerializer.Serialize(request, SerializationOptions.Current);

            var length = requestJson.Length;

            using (var writer = new BinaryWriter(stream, Encoding.Default, true))
            {
                writer.Write(length);
                writer.Write(requestJson);
            }
        }
    }
}