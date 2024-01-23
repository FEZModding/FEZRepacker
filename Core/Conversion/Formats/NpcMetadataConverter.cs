using FEZRepacker.Core.Definitions.Game.NpcMetadata;
using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.Helpers.Json;

namespace FEZRepacker.Core.Conversion.Formats
{
    internal class NpcMetadataConverter : FormatConverter<NpcMetadata>
    {
        public override string FileFormat => ".feznpc";

        public override FileBundle ConvertTyped(NpcMetadata data)
        {
            return ConfiguredJsonSerializer.SerializeToFileBundle(FileFormat, data);
        }

        public override NpcMetadata DeconvertTyped(FileBundle bundle)
        {
            return ConfiguredJsonSerializer.DeserializeFromFileBundle<NpcMetadata>(bundle);
        }
    }
}
