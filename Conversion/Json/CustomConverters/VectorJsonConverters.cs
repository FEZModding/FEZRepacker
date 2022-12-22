using System.Numerics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FEZRepacker.Conversion.Json.CustomConverters
{
    public class Vector3JsonConverter : JsonConverter<Vector3>
    {
        public override Vector3 Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if(reader.TokenType != JsonTokenType.StartArray) throw new JsonException();

            reader.Read();
            float x = reader.GetSingle();
            reader.Read();
            float y = reader.GetSingle();
            reader.Read();
            float z = reader.GetSingle();
            Vector3 v = new Vector3(x,y,z);
            reader.Read();

            if (reader.TokenType != JsonTokenType.EndArray) throw new JsonException();
            
            return v;
        }

        public override void Write(
            Utf8JsonWriter writer,
            Vector3 vector,
            JsonSerializerOptions options)
        {
            writer.WriteRawValue($"[{vector.X}, {vector.Y}, {vector.Z}]");
        }
    }

    public class Vector2JsonConverter : JsonConverter<Vector2>
    {
        public override Vector2 Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();

            reader.Read();
            float x = reader.GetSingle();
            reader.Read();
            float y = reader.GetSingle();
            Vector2 v = new Vector2(x,y);
            reader.Read();

            if (reader.TokenType != JsonTokenType.EndArray) throw new JsonException();

            return v;
        }

        public override void Write(
            Utf8JsonWriter writer,
            Vector2 vector,
            JsonSerializerOptions options)
        {
            writer.WriteRawValue($"[{vector.X}, {vector.Y}]");
        }
    }
}
