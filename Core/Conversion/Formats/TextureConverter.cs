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
        private const string FileFormat = ".png";
        private const string SurfaceTypeMetadataKey = "XNASurfaceType";
        
        public override string[] FileFormats => [FileFormat];

        public override FileBundle ConvertTyped(Texture2D texture)
        {
            using var image = TexturesUtil.ImageFromTexture2D(texture);
            StoreSurfaceFormatInImage(image, texture.Format);
            var outStream = image.SaveAsMemoryStream(new PngEncoder());
            return FileBundle.Single(outStream, FileFormat);
        }

        public override Texture2D DeconvertTyped(FileBundle bundle)
        {
            using var importedImage = Image.Load<Rgba32>(bundle.RequireData(""));
            var format = TryDetectDxtFormat(importedImage, SurfaceFormat.Color);
            return TexturesUtil.ImageToTexture2D(importedImage, format);
        }
        
        private static void StoreSurfaceFormatInImage(Image<Rgba32> img, SurfaceFormat format)
        {
            PngMetadata pngMeta = img.Metadata.GetPngMetadata();
            pngMeta.TextData.Add(new PngTextData(SurfaceTypeMetadataKey, format.ToString(), "", ""));
        }
        
        private static SurfaceFormat TryDetectDxtFormat(Image<Rgba32> img, SurfaceFormat defaultFormat)
        {
            PngMetadata pngMeta = img.Metadata.GetPngMetadata();
            var surfaceTypeMetadata = pngMeta.TextData.FirstOrDefault(data => data.Keyword == SurfaceTypeMetadataKey);
            if (surfaceTypeMetadata != null && Enum.TryParse(surfaceTypeMetadata.Value, out SurfaceFormat surfaceFormat))
            {
                return surfaceFormat;
            }
            
            return defaultFormat;
        }
    }
}
