using FEZRepacker.Core.Definitions.Game.Level;
using FEZRepacker.Core.Definitions.Json;
using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.Helpers.Json;

namespace FEZRepacker.Core.Conversion
{
    internal class LevelConverter : FormatConverter<Level>
    {
        public override string FileFormat => ".fezlvl";

        public override FileBundle ConvertTyped(Level data)
        {
            var levelModel = new LevelJsonModel(data);
            return ConfiguredJsonSerializer.SerializeToFileBundle(FileFormat, levelModel);
        }

        public override Level DeconvertTyped(FileBundle bundle)
        {
            var levelModel = ConfiguredJsonSerializer.DeserializeFromFileBundle<LevelJsonModel>(bundle);
            return levelModel.Deserialize();
        }
    }
}
