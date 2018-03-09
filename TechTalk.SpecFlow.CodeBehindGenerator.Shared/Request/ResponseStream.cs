using System.IO;
using Newtonsoft.Json;
using TechTalk.SpecFlow.CodeBehindGenerator.Shared.Response;

namespace TechTalk.SpecFlow.CodeBehindGenerator.Shared.Request
{
    public class ResponseStream
    {
        public static T Read<T>(Stream stream) where T : BaseResponse
        {

            var jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
            });


            using (var streamReader = new StreamReader(stream))
            {
                using (var textWriter = new JsonTextReader(streamReader))
                {
                    return jsonSerializer.Deserialize<T>(textWriter);
                }
            }
        }

        public static void Write<T>(T request, Stream stream) where T : BaseResponse
        {
            var jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
            });


            using (var streamWriter = new StreamWriter(stream))
            {
                using (var textWriter = new JsonTextWriter(streamWriter))
                {
                    jsonSerializer.Serialize(textWriter, request);
                }
            }
        }
    }
}