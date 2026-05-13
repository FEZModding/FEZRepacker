using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.Helpers;
using FEZRepacker.Core.Helpers.Json;

namespace FEZRepacker.Core.Conversion.Formats
{
    using TextStorage = OrderedDictionary<string, OrderedDictionary<string, string>>;
    internal class TextStorageConverter : FormatConverter<TextStorage>
    {
        private const string BundleFileFormat = ".feztxt";
        
        public override string[] FileFormats => [BundleFileFormat];

        public override FileBundle ConvertTyped(TextStorage data)
        {
            return ConfiguredJsonSerializer.SerializeToFileBundle(BundleFileFormat, data);
        }

        public override TextStorage DeconvertTyped(FileBundle bundle)
        {
            return ConfiguredJsonSerializer.DeserializeFromFileBundle<TextStorage>(bundle);
        }
    }
}
