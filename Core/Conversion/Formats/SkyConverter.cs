using FEZRepacker.Core.Definitions.Game.Sky;
using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.Helpers.Json;

namespace FEZRepacker.Core.Conversion.Formats
{
    internal class SkyConverter : FormatConverter<Sky>
    {
        private const string BundleFileFormat = ".fezsky";
        public override string[] FileFormats => [BundleFileFormat];

        public override FileBundle ConvertTyped(Sky data)
        {
            return ConfiguredJsonSerializer.SerializeToFileBundle(BundleFileFormat, data);
        }

        public override Sky DeconvertTyped(FileBundle bundle)
        {
            return ConfiguredJsonSerializer.DeserializeFromFileBundle<Sky>(bundle);
        }
    }
}
