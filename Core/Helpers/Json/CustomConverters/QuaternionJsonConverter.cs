
using System.Text.Json;
using System.Text.Json.Serialization;

using FEZRepacker.Core.Definitions.Game.XNA;

namespace FEZRepacker.Core.Helpers.Json.CustomConverters
{
    internal class QuaternionJsonConverter : JsonConverter<Quaternion>
    {
        public override Quaternion Read(
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
            reader.Read();
            float w = reader.GetSingle();
            Quaternion q = new Quaternion(x, y, z, w);
            reader.Read();

            if (reader.TokenType != JsonTokenType.EndArray) throw new JsonException();

            return q;
        }

        public override void Write(
            Utf8JsonWriter writer,
            Quaternion quaternion,
            JsonSerializerOptions options)
        {
            // Preserve decimal places in JSON number by converting float value to decimal
            writer.WriteStartArray();
            writer.WriteNumberValue((decimal)quaternion.X);
            writer.WriteNumberValue((decimal)quaternion.Y);
            writer.WriteNumberValue((decimal)quaternion.Z);
            writer.WriteNumberValue((decimal)quaternion.W);
            writer.WriteEndArray();
        }
    }
}
