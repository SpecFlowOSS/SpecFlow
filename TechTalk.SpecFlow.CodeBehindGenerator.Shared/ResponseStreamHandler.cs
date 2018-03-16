using System.IO;
using Newtonsoft.Json;
using TechTalk.SpecFlow.CodeBehindGenerator.Shared.Response;

namespace TechTalk.SpecFlow.CodeBehindGenerator.Shared
{
    public class ResponseStreamHandler
    {
        public static T Read<T>(Stream stream) where T : BaseResponse
        {

            using (var writer = new BinaryReader(stream))
            {
                var length = writer.ReadInt32();
                var requestJson = new string(writer.ReadChars(length));


                return JsonConvert.DeserializeObject<T>(requestJson, SerializationOptions.Current);
            }
        }

        public static void Write<T>(T request, Stream stream) where T : BaseResponse
        {
            var requestJson = JsonConvert.SerializeObject(request, SerializationOptions.Current);

            var length = requestJson.Length;

            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(length);
                writer.Write(requestJson.ToCharArray());
            }
        }
    }
}