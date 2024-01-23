using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

using FEZRepacker.Core.Definitions.Game.XNA;

namespace FEZRepacker.Core.Helpers.Json.CustomConverters
{
    internal class ColorJsonConverter : JsonConverter<Color>
    {
        public override Color Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String) throw new JsonException();

            string colorStr = JsonDocument.ParseValue(ref reader).Deserialize<string>() ?? "#00000000";
            return ColorFromHTML(colorStr);
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

        private static Color ColorFromHTML(string htmlCode)
        {
            string hexCode = htmlCode.Replace("#", "");

            byte[] values = { 0, 0, 0, 255 };
            int valueSize = hexCode.Length < 6 ? 1 : 2;
            if (valueSize == 2 && hexCode.Length % 2 == 1) hexCode += "0";
            int valuesCount = hexCode.Length / valueSize;
            for (int i = 0; i < valuesCount; i++)
            {
                string hexValue = hexCode.Substring(i * valueSize, valueSize);
                values[i] = byte.Parse(hexValue, NumberStyles.HexNumber);
            }

            return new Color(values[0], values[1], values[2], values[3]);
        }
    }
}
