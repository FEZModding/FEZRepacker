using System.Text.Json;
using System.Text.Json.Serialization;

namespace FEZRepacker.Converter.XNB.Formats.Json.CustomConverters
{
    internal class TimeSpanJsonConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            return TimeSpan.FromSeconds(reader.GetDouble());
        }

        public override void Write(
            Utf8JsonWriter writer,
            TimeSpan timespan,
            JsonSerializerOptions options)
        {
            writer.WriteRawValue($"{timespan.TotalSeconds}");
        }
    }
}
