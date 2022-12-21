using System.Numerics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using FEZEngine.Structure;

namespace FEZRepacker.Conversion.Json.CustomConverters
{
    public class TrileEmplacementListJsonConverter : JsonConverter<List<TrileEmplacement>>
    {

        public override List<TrileEmplacement> Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();

            List<TrileEmplacement> trilePoses = new();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray) return trilePoses;

                if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();

                TrileEmplacement trile = new TrileEmplacement(reader.GetInt32(), reader.GetInt32(), reader.GetInt32());

                reader.Read();
                if (reader.TokenType != JsonTokenType.EndArray) throw new JsonException();

                trilePoses.Add(trile);
            }

            throw new JsonException();
        }

        public override void Write(
            Utf8JsonWriter writer,
            List<TrileEmplacement> triles,
            JsonSerializerOptions options)
        {

            writer.WriteStartArray();

            foreach (var pos in triles)
            {
                if (options.WriteIndented)
                {
                    writer.WriteRawValue("\n" + new string(' ', writer.CurrentDepth * 2) + $"[{pos.X}, {pos.Y}, {pos.Z}]");
                }
            }

            writer.WriteEndArray();
        }
    }
}
