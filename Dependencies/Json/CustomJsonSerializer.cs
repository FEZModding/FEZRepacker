using System.Text.Json;
using System.Text.Json.Serialization;
using FEZRepacker.Dependencies.Json.CustomConverters;

namespace FEZRepacker.Dependencies.Json
{
    public static class CustomJsonSerializer
    {
        private static JsonSerializerOptions? _serializerOptions;

        private static void PrepareOptions()
        {
            if (_serializerOptions != null) return;
            _serializerOptions = new JsonSerializerOptions
            {
                IncludeFields = true,
                WriteIndented = true,
                Converters = {
                    new JsonStringEnumConverter(),
                    new VectorJsonConverter(),
                    new TripleEmplacementJsonConverter(),
                    new ScriptTriggerJsonConverter(),
                    new ScriptConditionJsonConverter(),
                    new ScriptActionJsonConverter(),
                }
            };
        }

        public static string Serialize<T>(T serializable)
        {
            PrepareOptions();
            return JsonSerializer.Serialize(serializable, _serializerOptions);
        }

        public static T Deserialize<T>(string json)
        {
            PrepareOptions();
            return JsonSerializer.Deserialize<T>(json, _serializerOptions)!;
        }
        
    }
}
