using System;
using Newtonsoft.Json;

namespace TechTalk.SpecFlow.Rpc.Shared
{
    public class SerializationOptions
    {
        private static readonly Lazy<JsonSerializerSettings> _current = new Lazy<JsonSerializerSettings>(() => new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
        });

        public static JsonSerializerSettings Current => _current.Value;
    }
}