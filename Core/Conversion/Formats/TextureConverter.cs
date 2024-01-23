using FEZRepacker.Core.Definitions.Game.XNA;
using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.Helpers;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace FEZRepacker.Core.Conversion.Formats
{
    internal class TextureConverter : FormatConverter<Texture2D>
    {
        public override string FileFormat => ".png";

        public override FileBundle ConvertTyped(Texture2D texture)
        {
            using var image = TexturesUtil.ImageFromTexture2D(texture);
            var outStream = image.SaveAsMemoryStream(new PngEncoder());
            return FileBundle.Single(outStream, FileFormat);
        }

        public override Texture2D DeconvertTyped(FileBundle bundle)
        {
            using var importedImage = Image.Load<Rgba32>(bundle.GetData(""));
            return TexturesUtil.ImageToTexture2D(importedImage);
        }
    }
}
