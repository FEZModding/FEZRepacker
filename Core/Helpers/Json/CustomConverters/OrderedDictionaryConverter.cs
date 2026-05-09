using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FEZRepacker.Core.Helpers.Json.CustomConverters
{
    public class OrderedDictionaryConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType)
            {
                return false;
            }

            return typeToConvert.GetGenericTypeDefinition() == typeof(IDictionary<,>);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            Type[] typeArgs = typeToConvert.GetGenericArguments();
            var keyType = typeArgs[0];
            var valueType = typeArgs[1];
            var converterType = typeof(OrderedDictionaryConverter<,>).MakeGenericType(keyType, valueType);

            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }
    }
    
    public class OrderedDictionaryConverter<TKey, TValue> 
        : JsonConverter<IDictionary<TKey, TValue>> where TKey : notnull 
    {
        public override IDictionary<TKey, TValue> Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

            var keyConverter = TypeDescriptor.GetConverter(typeof(TKey));
            if (!keyConverter.CanConvertFrom(typeof(string))) throw new JsonException();
            
            var result = new OrderedDictionary<TKey, TValue>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject) return result;

                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();

                TKey key = (TKey)keyConverter.ConvertFromInvariantString(reader.GetString()!)!;

                reader.Read();

                TValue value = JsonSerializer.Deserialize<TValue>(ref reader, options)!;

                result.Add(key, value);
            }

            throw new JsonException();
        }

        public override void Write(
            Utf8JsonWriter writer,
            IDictionary<TKey, TValue> value,
            JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            foreach (var kvp in value)
            {
                writer.WritePropertyName(JsonSerializer.Serialize(kvp.Key, options).Trim('"'));
                JsonSerializer.Serialize(writer, kvp.Value, options);
            }
            writer.WriteEndObject();
        }
    }
}
