using System;

using Utf8Json;
using Utf8Json.Resolvers;

namespace TechTalk.SpecFlow.Rpc.Shared
{
    public class SerializationOptions
    {
        //private static readonly Lazy<JsonSerializerSettings> _current = new Lazy<JsonSerializerSettings>(() => new JsonSerializerSettings()
        //{
        //    TypeNameHandling = TypeNameHandling.Auto,
        //    MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
        //});

        //public static JsonSerializerSettings Current => _current.Value;

        private static readonly Lazy<IJsonFormatterResolver> _current = new Lazy<IJsonFormatterResolver>(() =>
        {
            
            return StandardResolver.Default;
        });

        public static IJsonFormatterResolver Current => _current.Value;
    }
}