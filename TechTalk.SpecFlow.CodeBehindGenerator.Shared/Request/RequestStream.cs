using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TechTalk.SpecFlow.CodeBehindGenerator.Shared.Request
{
    public class RequestStream
    {
        public static void Write<T>(T request, Stream stream) where T : BaseRequest
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

        public static T Read<T>(Stream stream) where T : BaseRequest
        {
            var jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
            });


            using (var streamReader = new StreamReader(stream))
            {
                using (var textReader = new JsonTextReader(streamReader))
                {
                    return jsonSerializer.Deserialize<T>(textReader);
                }
            }
        }
    }
}
