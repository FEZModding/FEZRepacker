using System.Text.Json;
using System.Text.Json.Serialization;

namespace FEZRepacker.Core.Helpers.Json.CustomConverters
{
    internal class TimeSpanJsonConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            return TimeSpan.FromTicks((long)Math.Round(reader.GetDecimal() * TimeSpan.TicksPerSecond));
        }

        public override void Write(
            Utf8JsonWriter writer,
            TimeSpan timespan,
            JsonSerializerOptions options)
        {
            writer.WriteNumberValue((decimal)timespan.Ticks / TimeSpan.TicksPerSecond);
        }
    }
}
