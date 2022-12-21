using System.Numerics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using FEZEngine.Structure;

namespace FEZRepacker.Conversion.Json.CustomConverters
{
    public class TrileEmplacementJsonConverter : JsonConverter<TrileEmplacement>
    {
        public override TrileEmplacement Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if(reader.TokenType != JsonTokenType.StartArray) throw new JsonException();

            TrileEmplacement trile = new TrileEmplacement(reader.GetInt32(), reader.GetInt32(), reader.GetInt32());

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
