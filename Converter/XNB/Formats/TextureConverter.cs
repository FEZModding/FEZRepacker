using System.Runtime.InteropServices;

using FEZRepacker.Converter.Definitions.MicrosoftXna;
using FEZRepacker.Converter.XNB;
using FEZRepacker.Converter.XNB.Types;
using FEZRepacker.Converter.XNB.Types.System;

using Microsoft.Xna.Framework.Graphics;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace FEZRepacker.Converter.XNB.Formats
{
    internal class TextureConverter : XnbFormatConverter
    {
        public override List<XnbContentType> TypesFactory => new List<XnbContentType>
        {
            new GenericContentType<Texture2D>(this),
            new EnumContentType<SurfaceFormat>(this),
            new ByteArrayContentType(this)
        };
        public override string FileFormat => ".png";

        public static Image<Rgba32> ImageFromTexture2D(Texture2D txt)
        {
            // most of FEZ textures are saved in raw format
            byte[] textureData;

            // some of them are not. try to convert them.
            switch (txt.Format)
            {
                case SurfaceFormat.Color:
                    textureData = txt.TextureData;
                    break;
                case SurfaceFormat.Dxt1:
                    textureData = DxtUtil.DecompressDxt1(txt.TextureData, txt.Width, txt.Height);
                    break;
                case SurfaceFormat.Dxt3:
                    textureData = DxtUtil.DecompressDxt3(txt.TextureData, txt.Width, txt.Height);
                    break;
                case SurfaceFormat.Dxt5:
                    textureData = DxtUtil.DecompressDxt5(txt.TextureData, txt.Width, txt.Height);
                    break;
                default:
                    throw new InvalidDataException($"Texture2D has unsupported format ({txt.Format})");
            }

            return Image.LoadPixelData<Rgba32>(textureData, txt.Width, txt.Height);
        }

        public static Texture2D ImageToTexture2D(Image<Rgba32> img)
        {
            Texture2D texture = new Texture2D();
            texture.Format = SurfaceFormat.Color;
            texture.MipmapLevels = 1;
            texture.Width = img.Width;
            texture.Height = img.Height;

            texture.TextureData = new byte[img.Width * img.Height * 4];
            img.CopyPixelDataTo(texture.TextureData);

            return texture;
        }


        public override void FromBinary(BinaryReader xnbReader, BinaryWriter outWriter)
        {
            Texture2D texture = (Texture2D)PrimaryContentType.Read(xnbReader);

            using var image = ImageFromTexture2D(texture);

            image.Save(outWriter.BaseStream, new PngEncoder());
        }

        public override void ToBinary(BinaryReader inReader, BinaryWriter xnbWriter)
        {
            using var importedImage = Image.Load<Rgba32>(inReader.BaseStream);

            Texture2D texture = ImageToTexture2D(importedImage);

            PrimaryContentType.Write(texture, xnbWriter);
        }
    }
}
