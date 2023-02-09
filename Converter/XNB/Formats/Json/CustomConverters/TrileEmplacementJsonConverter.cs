using System.Text.Json;
using System.Text.Json.Serialization;

using FEZRepacker.Converter.Definitions.FezEngine.Structure;

namespace FEZRepacker.Converter.XNB.Formats.Json.CustomConverters
{
    internal class TrileEmplacementJsonConverter : JsonConverter<TrileEmplacement>
    {
        public override TrileEmplacement Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();

            reader.Read();
            int x = reader.GetInt32();
            reader.Read();
            int y = reader.GetInt32();
            reader.Read();
            int z = reader.GetInt32();
            TrileEmplacement trile = new TrileEmplacement(x, y, z);
            reader.Read();

            if (reader.TokenType != JsonTokenType.EndArray) throw new JsonException();

            return trile;
        }

        public override void Write(
            Utf8JsonWriter writer,
            TrileEmplacement trilePos,
            JsonSerializerOptions options)
        {
            writer.WriteRawValue($"[{trilePos.X}, {trilePos.Y}, {trilePos.Z}]");
        }
    }
}
