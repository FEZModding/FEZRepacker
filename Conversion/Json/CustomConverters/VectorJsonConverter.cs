using System.Numerics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FEZRepacker.Conversion.Json.CustomConverters
{
    public class VectorJsonConverter : JsonConverter<Vector3>
    {
        public override Vector3 Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if(reader.TokenType != JsonTokenType.StartArray) throw new JsonException();

            Vector3 v = new Vector3(reader.GetSingle(), reader.GetSingle(), reader.GetSingle());

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
}
