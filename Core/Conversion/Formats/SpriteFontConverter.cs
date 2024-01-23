using FEZRepacker.Core.Definitions.Game.XNA;
using FEZRepacker.Core.Definitions.Json;
using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.Helpers;
using FEZRepacker.Core.Helpers.Json;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace FEZRepacker.Core.Conversion.Formats
{
    internal class SpriteFontConverter : FormatConverter<SpriteFont>
    {
        public override string FileFormat => ".fezfont";

        public override FileBundle ConvertTyped(SpriteFont data)
        {
            var spriteFontModel = new SpriteFontPropertiesJsonModel();
            spriteFontModel.SerializeFrom(data);
            var bundle = ConfiguredJsonSerializer.SerializeToFileBundle(FileFormat, spriteFontModel);

            using var fontAtlas = TexturesUtil.ImageFromTexture2D(data.Texture);
            bundle.AddFile(fontAtlas.SaveAsMemoryStream(new PngEncoder()), ".png");

            return bundle;
        }

        public override SpriteFont DeconvertTyped(FileBundle bundle)
        {
            var spriteFontModel = ConfiguredJsonSerializer.DeserializeFromFileBundle<SpriteFontPropertiesJsonModel>(bundle);
            var spriteFont = spriteFontModel.Deserialize();

            using var importedImage = Image.Load<Rgba32>(bundle.GetData(".png"));
            spriteFont.Texture = TexturesUtil.ImageToTexture2D(importedImage);

            return spriteFont;
        }
    }
}
