using System.Numerics;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

using FEZEngine.Structure;

namespace FEZRepacker.Dependencies.Yaml.CustomConverters
{
    class TrileEmplacementYamlTypeConverter : IYamlTypeConverter
    {
        const string PREFIX = "";
        const string SUFFIX = "";
        const string SEPARATOR = " ";

        public bool Accepts(Type type)
        {
            return type == typeof(TrileEmplacement);
        }

        public object? ReadYaml(IParser parser, Type type)
        {
            var value = parser.Consume<Scalar>().Value;

            if (value == null) return null;
            if (!value.StartsWith(PREFIX) || !value.EndsWith(SUFFIX)) return null;

            var numsStr = value.Substring(PREFIX.Length, value.Length - PREFIX.Length - SUFFIX.Length).Split(SEPARATOR);
            var nums = Array.ConvertAll(numsStr, int.Parse);

            TrileEmplacement vec = new TrileEmplacement();
            if (nums.Length > 0) vec.X = nums[0];
            if (nums.Length > 1) vec.Y = nums[1];
            if (nums.Length > 2) vec.Z = nums[2];
            return vec;
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type)
        {
            if (value == null) return;

            TrileEmplacement pos = (TrileEmplacement)value;
            string output = $"{PREFIX}{pos.X}{SEPARATOR}{pos.Y}{SEPARATOR}{pos.Z}{SUFFIX}";

            emitter.Emit(new Scalar(TagName.Empty, output));
        }
    }
}
