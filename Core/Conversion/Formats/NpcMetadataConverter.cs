using FEZRepacker.Core.Definitions.Game.NpcMetadata;
using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.Helpers.Json;

namespace FEZRepacker.Core.Conversion.Formats
{
    internal class NpcMetadataConverter : FormatConverter<NpcMetadata>
    {
        private const string BundleFileFormat = ".feznpc";
        public override string[] FileFormats => [BundleFileFormat];

        public override FileBundle ConvertTyped(NpcMetadata data)
        {
            return ConfiguredJsonSerializer.SerializeToFileBundle(BundleFileFormat, data);
        }

        public override NpcMetadata DeconvertTyped(FileBundle bundle)
        {
            return ConfiguredJsonSerializer.DeserializeFromFileBundle<NpcMetadata>(bundle);
        }
    }
}
