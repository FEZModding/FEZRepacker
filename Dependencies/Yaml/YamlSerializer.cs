using YamlDotNet.Serialization;

using FEZRepacker.Dependencies.Yaml.CustomConfigures;
using YamlDotNet.Serialization.EventEmitters;

namespace FEZRepacker.Dependencies.Yaml
{
    static class YamlSerializer
    {
        private static readonly IYamlTypeConverter[] _typeConverters = new IYamlTypeConverter[]
        {
            new VectorYamlTypeConverter(),
            new TrileEmplacementYamlTypeConverter(),
            new ScriptPropertiesYamlTypeConverter(),
        };

        private static ISerializer? _serializer = null;
        private static IDeserializer? _deserializer = null;

        private static ISerializer Serializer()
        {
            if(_serializer == null)
            {
                var builder = new SerializerBuilder()
                    .WithEventEmitter(next => new FlowStyleEnumSequences(next))
                    .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull);
                
                foreach(var type in _typeConverters)
                {
                    builder = builder.WithTypeConverter(type);
                }
                _serializer = builder.Build();
            }

            return _serializer;
        }

        private static IDeserializer Deserializer()
        {
            if(_deserializer == null)
            {
                var builder = new DeserializerBuilder();

                foreach (var type in _typeConverters)
                {
                    builder = builder.WithTypeConverter(type);
                }
                _deserializer = builder.Build();
            }
            return _deserializer;
        }

        public static string Serialize(object obj)
        {
            return Serializer().Serialize(obj);
        }

        public static T Deserialize<T>(string yaml)
        {
            return Deserializer().Deserialize<T>(yaml);
        }
    }
}
