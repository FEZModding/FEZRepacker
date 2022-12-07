using System.Numerics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FEZRepacker.Dependencies.Json.CustomConverters
{
    public class QuaternionJsonConverter : JsonConverter<Quaternion>
    {
        public override Quaternion Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if(reader.TokenType != JsonTokenType.StartArray) throw new JsonException();

            Quaternion q = new Quaternion(reader.GetSingle(), reader.GetSingle(), reader.GetSingle(), reader.GetSingle());

            reader.Read();
            if (reader.TokenType != JsonTokenType.EndArray) throw new JsonException();
            
            return q;
        }

        public override void Write(
            Utf8JsonWriter writer,
            Quaternion quaternion,
            JsonSerializerOptions options)
        {
            writer.WriteRawValue($"[{quaternion.X}, {quaternion.Y}, {quaternion.Z}, {quaternion.W}]");
        }
    }
}
