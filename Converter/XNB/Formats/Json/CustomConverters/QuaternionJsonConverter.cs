using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FEZRepacker.Converter.XNB.Formats.Json.CustomConverters
{
    internal class QuaternionJsonConverter : JsonConverter<Quaternion>
    {
        public override Quaternion Read(
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
            reader.Read();
            float w = reader.GetSingle();
            Quaternion q = new Quaternion(x,y,z,w);
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
