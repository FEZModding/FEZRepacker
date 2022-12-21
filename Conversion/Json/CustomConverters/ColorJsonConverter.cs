using System.Numerics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Drawing;

namespace FEZRepacker.Conversion.Json.CustomConverters
{
    public class ColorJsonConverter : JsonConverter<Color>
    {
        public override Color Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if(reader.TokenType != JsonTokenType.String) throw new JsonException();

            string colorStr = JsonDocument.ParseValue(ref reader).Deserialize<string>() ?? "#00000000";

            return ColorTranslator.FromHtml(colorStr);
        }

        public override void Write(
            Utf8JsonWriter writer,
            Color color,
            JsonSerializerOptions options)
        {
            string rHex = color.R.ToString("X2");
            string gHex = color.G.ToString("X2");
            string bHex = color.B.ToString("X2");
            string aHex = color.A.ToString("X2");

            writer.WriteRawValue($"\"#{rHex}{gHex}{bHex}{aHex}\"");
        }
    }
}
