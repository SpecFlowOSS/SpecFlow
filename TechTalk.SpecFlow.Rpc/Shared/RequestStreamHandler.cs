﻿using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace TechTalk.SpecFlow.Rpc.Shared
{
    public class RequestStreamHandler
    {
        public static void Write<T>(T request, Stream stream) where T : Request.Request
        {
            // TODO: unit testing
            var requestJson = JsonConvert.SerializeObject(request, SerializationOptions.Current);

            var length = requestJson.Length;

            using (var writer = new BinaryWriter(stream, Encoding.Default, true))
            {
                writer.Write(length);
                writer.Write(requestJson.ToCharArray());
            }
        }

        public static T Read<T>(Stream stream) where T : Request.Request
        {
            // TODO: unit testing
            using (var reader = new BinaryReader(stream, Encoding.Default, true))
            {
                var length = reader.ReadInt32();
                var requestJson = new string(reader.ReadChars(length));

                return JsonConvert.DeserializeObject<T>(requestJson, SerializationOptions.Current);
            }
        }
    }
}
