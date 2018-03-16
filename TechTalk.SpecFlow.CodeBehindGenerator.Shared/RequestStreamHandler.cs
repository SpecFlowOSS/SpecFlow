using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TechTalk.SpecFlow.CodeBehindGenerator.Shared.Request;

namespace TechTalk.SpecFlow.CodeBehindGenerator.Shared
{
    public class RequestStreamHandler
    {
        public static void Write<T>(T request, Stream stream) where T : BaseRequest
        {
            var requestJson = JsonConvert.SerializeObject(request, SerializationOptions.Current);

            var length = requestJson.Length;

            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(length);
                writer.Write(requestJson.ToCharArray());
            }
        }

        public static T Read<T>(Stream stream) where T : BaseRequest
        {
            using (var writer = new BinaryReader(stream))
            {
                var length = writer.ReadInt32();
                var requestJson = new string(writer.ReadChars(length));


                return JsonConvert.DeserializeObject<T>(requestJson, SerializationOptions.Current);
            }
        }
    }
}
