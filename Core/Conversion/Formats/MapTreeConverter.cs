using FEZRepacker.Core.Definitions.Game.MapTree;
using FEZRepacker.Core.Definitions.Json;
using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.Helpers.Json;

namespace FEZRepacker.Core.Conversion.Formats
{
    internal class MapTreeConverter : FormatConverter<MapTree>
    {
        private const string BundleFileFormat = ".fezmap";
        public override string[] FileFormats => [BundleFileFormat];

        public override FileBundle ConvertTyped(MapTree data)
        {
            var mapModel = new MapTreeJsonModel();
            mapModel.SerializeFrom(data);
            return ConfiguredJsonSerializer.SerializeToFileBundle(BundleFileFormat, mapModel);
        }

        public override MapTree DeconvertTyped(FileBundle bundle)
        {
            var mapModel = ConfiguredJsonSerializer.DeserializeFromFileBundle<MapTreeJsonModel>(bundle);
            return mapModel.Deserialize();
        }
    }
}
