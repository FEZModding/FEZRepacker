using System.Numerics;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace FEZRepacker.Dependencies.Yaml.CustomConverters
{
    class VectorYamlTypeConverter : IYamlTypeConverter
    {
        const string PREFIX = "";
        const string SUFFIX = "";
        const string SEPARATOR = " ";

        public bool Accepts(Type type)
        {
            return type == typeof(Vector3) || type == typeof(Vector2);
        }

        public object? ReadYaml(IParser parser, Type type)
        {
            var value = parser.Consume<Scalar>().Value;

            if (value == null) return null;
            if (!value.StartsWith(PREFIX) || !value.EndsWith(SUFFIX)) return null;

            var numsStr = value.Substring(PREFIX.Length, value.Length - PREFIX.Length - SUFFIX.Length).Split(SEPARATOR);
            var nums = Array.ConvertAll(numsStr, float.Parse);

            if (type == typeof(Vector2))
            {
                Vector2 vec = new Vector2();
                if (nums.Length > 0) vec.X = nums[0];
                if (nums.Length > 1) vec.Y = nums[1];
                return vec;
            }
            else if (type == typeof(Vector3))
            {
                Vector3 vec = new Vector3();
                if (nums.Length > 0) vec.X = nums[0];
                if (nums.Length > 1) vec.Y = nums[1];
                if (nums.Length > 2) vec.Z = nums[2];
                return vec;
            }
            else return null;
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type)
        {
            if (value == null) return;

            string output = $"{PREFIX}{SUFFIX}";
            if(type == typeof(Vector2))
            {
                Vector2 vec = (Vector2)value;
                output = $"{PREFIX}{vec.X}{SEPARATOR}{vec.Y}{SUFFIX}";
            }
            if(type == typeof(Vector3))
            {
                Vector3 vec = (Vector3)value;
                output = $"{PREFIX}{vec.X}{SEPARATOR}{vec.Y}{SEPARATOR}{vec.Z}{SUFFIX}";
            }

            emitter.Emit(new Scalar(TagName.Empty, output));
        }
    }
}
