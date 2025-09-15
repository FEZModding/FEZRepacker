using System.Text.Json;
using System.Text.Json.Serialization;

using FEZRepacker.Core.Definitions.Game.XNA;

namespace FEZRepacker.Core.Helpers.Json.CustomConverters
{
    internal class Vector3JsonConverter : JsonConverter<Vector3>
    {
        public override Vector3 Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();

            reader.Read();
            float x = reader.GetSingle();
            reader.Read();
            float y = reader.GetSingle();
            reader.Read();
            float z = reader.GetSingle();
            Vector3 v = new Vector3(x, y, z);
            reader.Read();

            if (reader.TokenType != JsonTokenType.EndArray) throw new JsonException();

            return v;
        }

        public override void Write(
            Utf8JsonWriter writer,
            Vector3 vector,
            JsonSerializerOptions options)
        {
            // Preserve decimal places in JSON number by converting float value to decimal
            writer.WriteStartArray();
            writer.WriteNumberValue((decimal)vector.X);
            writer.WriteNumberValue((decimal)vector.Y);
            writer.WriteNumberValue((decimal)vector.Z);
            writer.WriteEndArray();
        }
    }

    internal class Vector2JsonConverter : JsonConverter<Vector2>
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
            Vector2 v = new Vector2(x, y);
            reader.Read();

            if (reader.TokenType != JsonTokenType.EndArray) throw new JsonException();

            return v;
        }

        public override void Write(
            Utf8JsonWriter writer,
            Vector2 vector,
            JsonSerializerOptions options)
        {
            // Preserve decimal places in JSON number by converting float value to decimal
            writer.WriteStartArray();
            writer.WriteNumberValue((decimal)vector.X);
            writer.WriteNumberValue((decimal)vector.Y);
            writer.WriteEndArray();
        }
    }
}
