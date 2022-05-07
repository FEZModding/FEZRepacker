using YamlDotNet.Serialization;

using FEZRepacker.Dependencies.Yaml.CustomConverters;

namespace FEZRepacker.Dependencies.Yaml
{
    static class YamlSerializer
    {
        private static readonly IYamlTypeConverter[] _customConverters = new IYamlTypeConverter[]{
            new VectorYamlTypeConverter(),
            new TrileEmplacementYamlTypeConverter(),
        };

        private static ISerializer? _serializer = null;
        private static IDeserializer? _deserializer = null;

        private static ISerializer Serializer()
        {
            if(_serializer == null)
            {
                SerializerBuilder builder = new SerializerBuilder();
                foreach (var converter in _customConverters)
                {
                    builder = builder.WithTypeConverter(converter);
                }
                _serializer = builder.Build();
            }

            return _serializer;
        }

        private static IDeserializer Deserializer()
        {
            if(_deserializer == null)
            {
                DeserializerBuilder builder = new DeserializerBuilder();
                foreach (var converter in _customConverters)
                {
                    builder = builder.WithTypeConverter(converter);
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
