using YamlDotNet.Serialization;

using FEZRepacker.Dependencies.Yaml.CustomConfigures;
using YamlDotNet.Serialization.EventEmitters;

namespace FEZRepacker.Dependencies.Yaml
{
    static class YamlSerializer
    {
        private static ISerializer? _serializer = null;
        private static IDeserializer? _deserializer = null;

        private static ISerializer Serializer()
        {
            if(_serializer == null)
            {
                _serializer = new SerializerBuilder()
                    .WithTypeConverter(new VectorYamlTypeConverter())
                    .WithTypeConverter(new TrileEmplacementYamlTypeConverter())
                    .WithEventEmitter(next => new FlowStyleEnumSequences(next))
                    .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
                    .Build();
            }

            return _serializer;
        }

        private static IDeserializer Deserializer()
        {
            if(_deserializer == null)
            {
                _deserializer = new DeserializerBuilder()
                    .WithTypeConverter(new VectorYamlTypeConverter())
                    .WithTypeConverter(new TrileEmplacementYamlTypeConverter())
                    .Build();
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
