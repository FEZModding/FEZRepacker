using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FEZRepacker.Core.Helpers.Json.CustomConverters
{
    public class RoundTripFloatConverter : JsonConverter<float>
    {
        public override float Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.GetSingle();

        public override void Write(Utf8JsonWriter writer, float value, JsonSerializerOptions options)
            => writer.WriteRawValue(value.ToString("R", CultureInfo.InvariantCulture));
    }
}