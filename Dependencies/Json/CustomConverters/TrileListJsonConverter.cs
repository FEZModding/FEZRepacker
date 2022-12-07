using System.Numerics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using FEZEngine.Structure;

namespace FEZRepacker.Dependencies.Json.CustomConverters
{
    using TrileJsonDict = Dictionary<string, TrileInstance>;
    public class TrileListJsonConverter : JsonConverter<Dictionary<TrileEmplacement,TrileInstance>>
    {
        private JsonConverter<TrileJsonDict>? innerConverter;

        private JsonConverter<TrileJsonDict> GetInnerConverter(JsonSerializerOptions options)
        {
            if (innerConverter == null)
            {
                innerConverter = (JsonConverter<TrileJsonDict>)options.GetConverter(typeof(TrileJsonDict));
            }
            return innerConverter;
        }

        public override Dictionary<TrileEmplacement, TrileInstance> Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            var trileJsonDict = GetInnerConverter(options).Read(ref reader, typeof(TrileJsonDict), options);

            var trileDict = new Dictionary<TrileEmplacement, TrileInstance>();

            if (trileJsonDict == null) return trileDict;

            foreach ((var key, var value) in trileJsonDict)
            {
                int[] coords = new int[3];
                var coordsStr = key.Split(' ');
                for(var i = 0; i < Math.Min(3,coordsStr.Length); i++)
                {
                    coords[i] = int.Parse(coordsStr[i]);
                }
                trileDict[new TrileEmplacement(coords[0], coords[1], coords[2])] = value;
            }

            return trileDict;
        }

        public override void Write(
            Utf8JsonWriter writer,
            Dictionary<TrileEmplacement, TrileInstance> triles,
            JsonSerializerOptions options)
        {

            var trileJsonDict = new TrileJsonDict();

            foreach ((var key, var value) in triles)
            {
                trileJsonDict[$"{key.X} {key.Y} {key.Z}"] = value;
            }

            GetInnerConverter(options).Write(writer, trileJsonDict, options);
        }
    }
}
