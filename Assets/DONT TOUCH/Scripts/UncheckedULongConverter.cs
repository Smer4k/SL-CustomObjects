using System;
using Newtonsoft.Json;

namespace DONT_TOUCH.Scripts
{
    public sealed class UncheckedULongConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) =>
            objectType == typeof(ulong) || objectType == typeof(ulong?);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return objectType == typeof(ulong?) ? null : (object)0UL;
            }
            
            long signedValue = Convert.ToInt64(reader.Value);
            return unchecked((ulong)signedValue);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue((ulong)value);
        }

        public override bool CanWrite => true;
    }
}