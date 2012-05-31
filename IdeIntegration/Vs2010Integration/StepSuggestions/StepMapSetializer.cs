using System;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Vs2010Integration.StepSuggestions
{
    internal class StepMapSetializer
    {
        private readonly JsonSerializer serializer = CreateSerializer();

        public StepMap Deserialize(TextReader reader)
        {
            return (StepMap)serializer.Deserialize(reader, typeof(StepMap));
        }

        public void Serialize(TextWriter writer, StepMap stepMap)
        {
            serializer.Serialize(writer, stepMap);
        }

        private class CustomContractResolver : DefaultContractResolver
        {
            public override JsonContract ResolveContract(Type type)
            {
                if (type == typeof(IBindingMethod))
                    return base.ResolveContract(typeof(BindingMethod));

                if (type == typeof(IBindingParameter))
                    return base.ResolveContract(typeof(BindingParameter));

                if (type == typeof(IBindingType))
                    return base.ResolveContract(typeof(BindingType));

                if (type == typeof(Version))
                    return new JsonStringContract(typeof(Version)) { Converter = new ToStringJsonConverter<Version>() };

                if (type == typeof(CultureInfo))
                    return new JsonStringContract(typeof(CultureInfo)) { Converter = new ToStringJsonConverter<CultureInfo>() };

                return base.ResolveContract(type);
            }
        }

        internal class ToStringJsonConverter<T> : JsonConverter where T : class
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                if (value == null)
                    writer.WriteNull();
                else
                    writer.WriteValue(value.ToString());
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                            JsonSerializer serializer)
            {
                T result;

                if (reader.TokenType == JsonToken.Null)
                    result = null;
                else if (reader.TokenType == JsonToken.String)
                    result = (T) Activator.CreateInstance(typeof (T), (string) reader.Value);
                else
                    throw new InvalidOperationException("Unable to convert value: " + reader.Value);

                return result;
            }

            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof (T));
            }
        }

        public static JsonSerializer CreateSerializer()
        {
            var serializer = new JsonSerializer();
            serializer.ContractResolver = new CustomContractResolver();
            
            return serializer;
        }
    }
}