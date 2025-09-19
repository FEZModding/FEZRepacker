using FEZRepacker.Core.Definitions.Game.XNA;

using Microsoft.Xna.Framework.Graphics;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Color = SixLabors.ImageSharp.Color;

namespace FEZRepacker.Core.Helpers
{
    internal static class TexturesUtil
    {
        public enum CubemapPart
        {
            Albedo,
            Emission
        }

        public static Image<Rgba32> ImageFromTexture2D(Texture2D txt)
        {
            var textureData = GetConvertedTextureData(txt);
            return Image.LoadPixelData<Rgba32>(textureData, txt.Width, txt.Height);
        }

        public static Texture2D ImageToTexture2D(Image<Rgba32> img)
        {
            Texture2D texture = new Texture2D()
            {
                Format = SurfaceFormat.Color,
                MipmapLevels = 1,
                Width = img.Width,
                Height = img.Height
            };

            texture.TextureData = new byte[img.Width * img.Height * 4];
            img.CopyPixelDataTo(texture.TextureData);

            return texture;
        }

        public static Image<Rgba32> ExtractCubemapPartFromTexture(Texture2D txt, CubemapPart part)
        {
            var image = ImageFromTexture2D(txt);

            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    var pixelRow = accessor.GetRowSpan(y);
                    foreach (ref Rgba32 pixel in pixelRow)
                    {
                        // FEZ stores emission in alpha channel
                        if (part == CubemapPart.Emission) pixel.R = pixel.G = pixel.B = pixel.A;
                        pixel.A = 255;
                    }
                }
            });

            return image;
        }

        public static Image<Rgba32> ConstructCubemap(Stream? imageAlbedoStream, Stream? imageEmissionStream)
        {
            if (imageAlbedoStream == null && imageEmissionStream == null) 
                return new Image<Rgba32>(1, 1, Color.Pink);

            var albedoImage = Image.Load<Rgba32>(imageAlbedoStream ?? imageEmissionStream);

            using var emissionImage =
                imageEmissionStream != null
                ? Image.Load<Rgba32>(imageEmissionStream)
                : new Image<Rgba32>(albedoImage.Width, albedoImage.Height, Color.Black);

            albedoImage.ProcessPixelRows(emissionImage, (albedoAccesor, emissionAccesor) =>
            {
                for (int y = 0; y < albedoAccesor.Height; y++)
                {
                    var albedoPixelRow = albedoAccesor.GetRowSpan(y);
                    var emissionPixelRow = emissionAccesor.GetRowSpan(y);
                    for (int x = 0; x < albedoPixelRow.Length; x++)
                    {
                        albedoPixelRow[x].A = emissionPixelRow[x].R;
                    }
                }
            });

            return albedoImage;
        }


        private static byte[] GetConvertedTextureData(Texture2D txt)
        {
            // Most of FEZ textures are saved in raw format (SurfaceFormat.Color)
            // but some of them are not - try to convert them.

            var convertedData = txt.Format switch
            {
                SurfaceFormat.Color => txt.TextureData,
                SurfaceFormat.Dxt1 => DxtUtil.DecompressDxt1(txt.TextureData, txt.Width, txt.Height),
                SurfaceFormat.Dxt3 => DxtUtil.DecompressDxt3(txt.TextureData, txt.Width, txt.Height),
                SurfaceFormat.Dxt5 => DxtUtil.DecompressDxt5(txt.TextureData, txt.Width, txt.Height),
                _ => null
            };

            if (convertedData == null)
            {
                throw new InvalidDataException($"Texture2D has unsupported format ({txt.Format})");
            }

            return convertedData;
        }
    }
}
