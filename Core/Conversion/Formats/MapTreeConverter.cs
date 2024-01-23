using FEZRepacker.Core.Definitions.Game.MapTree;
using FEZRepacker.Core.Definitions.Json;
using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.Helpers.Json;

namespace FEZRepacker.Core.Conversion.Formats
{
    internal class MapTreeConverter : FormatConverter<MapTree>
    {
        public override string FileFormat => ".fezmap";

        public override FileBundle ConvertTyped(MapTree data)
        {
            var mapModel = new MapTreeJsonModel();
            mapModel.SerializeFrom(data);
            return ConfiguredJsonSerializer.SerializeToFileBundle(FileFormat, mapModel);
        }

        public override MapTree DeconvertTyped(FileBundle bundle)
        {
            var mapModel = ConfiguredJsonSerializer.DeserializeFromFileBundle<MapTreeJsonModel>(bundle);
            return mapModel.Deserialize();
        }
    }
}
