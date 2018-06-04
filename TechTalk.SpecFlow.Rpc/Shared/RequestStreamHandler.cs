using System.IO;
using System.Text;
using Utf8Json;


namespace TechTalk.SpecFlow.Rpc.Shared
{
    public class RequestStreamHandler
    {
        public static void Write<T>(T request, Stream stream) where T : Request.Request
        {
            byte[] requestJson = JsonSerializer.Serialize(request,SerializationOptions.Current);

            var length = requestJson.Length;

            using (var writer = new BinaryWriter(stream, Encoding.Default, true))
            {
                writer.Write(length);
                writer.Write(requestJson);
            }
        }

        public static T Read<T>(Stream stream) where T : Request.Request
        {
            using (var reader = new BinaryReader(stream, Encoding.Default, true))
            {
                var length = reader.ReadInt32();
                var requestJson = new string(reader.ReadChars(length));

                return JsonSerializer.Deserialize<T>(requestJson, SerializationOptions.Current);
            }
        }
    }
}
