using FEZRepacker.Core.Definitions.Game.Helpers;
using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.Helpers.Json;

namespace FEZRepacker.Core.Conversion.Formats
{
    using TextStorageContainer = IDictionary<string, IDictionary<string, string>>;
    
    internal class TextStorageConverter : FormatConverter<TextStorage>
    {
        private const string BundleFileFormat = ".feztxt";
        
        public override string[] FileFormats => [BundleFileFormat];

        public override FileBundle ConvertTyped(TextStorage data)
        {
            return ConfiguredJsonSerializer.SerializeToFileBundle(BundleFileFormat, data.AllResources);
        }

        public override TextStorage DeconvertTyped(FileBundle bundle)
        {
            var allResources = ConfiguredJsonSerializer.DeserializeFromFileBundle<TextStorageContainer>(bundle);
            return new TextStorage {AllResources = allResources};
        }
    }
}
