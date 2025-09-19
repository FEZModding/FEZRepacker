using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.Helpers.Json.CustomConverters;

namespace FEZRepacker.Core.Helpers.Json
{
    /// <summary>
    /// Wrapper for <see cref="JsonSerializer"/> that passes custom options 
    /// tailored for format conversion. Additionally, supplies some helper
    /// methods for interacting directly with <see cref="FileBundle"/> 
    /// </summary>
    internal static class ConfiguredJsonSerializer
    {
        static JsonSerializerOptions? _serializerOptions;

        public static string Serialize<T>(T data)
        {
            return JsonSerializer.Serialize(data, GetOptions());
        }

        public static T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, GetOptions())!;
        }

        public static FileBundle SerializeToFileBundle<T>(string secondaryFileFormat, T data)
        {
            var json = Serialize(data);
            var outStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            return FileBundle.Single(outStream, secondaryFileFormat, ".json");
        }

        public static T DeserializeFromFileBundle<T>(FileBundle bundle)
        {
            using var inReader = new BinaryReader(bundle.RequireData(".json", ""), Encoding.UTF8, true);
            var json = new string(inReader.ReadChars((int)inReader.BaseStream.Length));

            try
            {
                return Deserialize<T>(json);
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException($"No valid JSON structure in a file bundle: {ex.Message}");
            }
        }

        public static JsonNode SerializeToNode<T>(T data)
        {
            return JsonSerializer.SerializeToNode(data, GetOptions())!;
        }

        public static T DeserializeFromNode<T>(JsonNode node)
        {
            return node.Deserialize<T>(GetOptions())!;
        }

        private static JsonSerializerOptions GetOptions()
        {
            if (_serializerOptions == null) _serializerOptions = new()
            {
                IncludeFields = true,
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters = {
                    new JsonStringEnumConverter(),
                    new Vector2JsonConverter(),
                    new Vector3JsonConverter(),
                    new QuaternionJsonConverter(),
                    new ColorJsonConverter(),
                    new TimeSpanJsonConverter(),
                    new TrileEmplacementJsonConverter(),
                    new TrileEmplacementListJsonConverter(),
                    new ScriptTriggerJsonConverter(),
                    new ScriptConditionJsonConverter(),
                    new ScriptActionJsonConverter(),
                }
            };

            return _serializerOptions;
        }
    }
}
