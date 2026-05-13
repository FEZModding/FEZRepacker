using FEZRepacker.Core.Definitions.Game.Level;
using FEZRepacker.Core.Definitions.Json;
using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.Helpers.Json;

namespace FEZRepacker.Core.Conversion.Formats
{
    internal class LevelConverter : FormatConverter<Level>
    {
        private const string BundleFileFormat = ".fezlvl";
        public override string[] FileFormats => [BundleFileFormat];

        public override FileBundle ConvertTyped(Level data)
        {
            var levelModel = new LevelJsonModel(data);
            return ConfiguredJsonSerializer.SerializeToFileBundle(BundleFileFormat, levelModel);
        }

        public override Level DeconvertTyped(FileBundle bundle)
        {
            var levelModel = ConfiguredJsonSerializer.DeserializeFromFileBundle<LevelJsonModel>(bundle);
            return levelModel.Deserialize();
        }
    }
}
