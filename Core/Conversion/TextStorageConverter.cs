using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.Helpers.Json;

namespace FEZRepacker.Core.Conversion
{
    using TextStorage = Dictionary<string, Dictionary<string, string>>;
    internal class TextStorageConverter : FormatConverter<TextStorage>
    {
        public override string FileFormat => ".feztxt";

        public override FileBundle ConvertTyped(TextStorage data)
        {
            return ConfiguredJsonSerializer.SerializeToFileBundle(FileFormat, data);
        }

        public override TextStorage DeconvertTyped(FileBundle bundle)
        {
            return ConfiguredJsonSerializer.DeserializeFromFileBundle<TextStorage>(bundle);
        }
    }
}
