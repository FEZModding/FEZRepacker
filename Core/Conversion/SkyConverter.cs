
using FEZRepacker.Core.Definitions.Game.Sky;
using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.Helpers.Json;

namespace FEZRepacker.Core.Conversion
{
    internal class SkyConverter : FormatConverter<Sky>
    {
        public override string FileFormat => ".fezsky";

        public override FileBundle ConvertTyped(Sky data)
        {
            return ConfiguredJsonSerializer.SerializeToFileBundle(FileFormat, data);
        }

        public override Sky DeconvertTyped(FileBundle bundle)
        {
            return ConfiguredJsonSerializer.DeserializeFromFileBundle<Sky>(bundle);
        }
    }
}
