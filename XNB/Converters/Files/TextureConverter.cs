using FEZRepacker.Definitions.MicrosoftXna;
using FEZRepacker.XNB.Types;
using FEZRepacker.XNB.Types.System;
using FEZRepacker.XNB.Types.XNA;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Textures;
using SixLabors.ImageSharp.Textures.Formats.Dds;
using SixLabors.ImageSharp.Textures.TextureFormats;
using System.Runtime.InteropServices;

namespace FEZRepacker.XNB.Converters.Files
{
    class TextureConverter : XNBContentConverter
    {
        public override XNBContentType[] TypesFactory => new XNBContentType[]
        {
            new GenericContentType<Texture2D>(this),
            new EnumContentType<SurfaceFormat>(this),
            new ByteArrayContentType(this)
        };
        public override string FileFormat => "png";

        public static Image<Rgba32> ImageFromTexture2D(Texture2D txt)
        {
            // most of FEZ textures are saved in raw format
            // some of them are not. try to convert them.
            if (txt.Format != SurfaceFormat.Color)
            {
                if (txt.Format == SurfaceFormat.Dxt1 || txt.Format == SurfaceFormat.Dxt5)
                {
                    // We can convert DXT1 and DXT5 compressed textures using ImageSharp.Textures,
                    // but it requires DDS header in the data.
                    // we're manually building it so the reader doesn't complain
                    byte[] newData = new byte[txt.TextureData.Length + 128];
                    var writer = new BinaryWriter(new MemoryStream(newData));

                    writer.Write(0x20534444); // magic value
                    writer.Write(124); // size of header structure
                    writer.Write(0x00081007); // flags: CAPS, PIXELFORMAT, WIDTH, HEIGHT and LINEARSIZE
                    writer.Write(txt.Height);
                    writer.Write(txt.Width);
                    writer.Write(txt.TextureData.Length); // linear size
                    writer.Write(0); // depth, 0 for 2d textures
                    writer.Write(1); // mipmap texture count, only one
                    writer.Seek(11 * 4, SeekOrigin.Current); // 11 DWORDs reserved for custom data. we leave it empty
                    writer.Write(32); // size of pixelformat structure
                    writer.Write(0x00000004); // pixelformat flags: FOURCC
                    writer.Write(new char[] { 'D', 'X', 'T' }); // compressed format code
                    writer.Write(txt.Format == SurfaceFormat.Dxt5 ? '5' : '1'); // last character of format code
                    writer.Seek(5 * 4, SeekOrigin.Current); // RBG mask data. we skip it in this case
                    writer.Write(0x00001000); // CAPS_TEXTURE flag
                    writer.Seek(4 * 4, SeekOrigin.Current); // cubic map flags and three reserved DWORDs, skip
                    writer.Write(txt.TextureData); // actual data.

                    // decode DXT data and write it into byte array
                    var decoder = new DdsDecoder();
                    var config = SixLabors.ImageSharp.Textures.Configuration.Default;
                    FlatTexture texture = (FlatTexture) decoder.DecodeTexture(config, new MemoryStream(newData));
                    return texture.MipMaps[0].GetImage().CloneAs<Rgba32>();
                }
                else
                {
                    throw new InvalidDataException($"Texture2D has unsupported format ({txt.Format})");
                }
            }
            else
            {
                return Image.LoadPixelData<Rgba32>(txt.TextureData, txt.Width, txt.Height);
            }
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
            Texture2D texture = (Texture2D)Types[0].Read(xnbReader);

            using var image = ImageFromTexture2D(texture);

            image.Save(outWriter.BaseStream, new PngEncoder());
        }

        public override void ToBinary(BinaryReader inReader, BinaryWriter xnbWriter)
        {
            using var importedImage = Image.Load<Rgba32>(inReader.BaseStream);

            Texture2D texture = ImageToTexture2D(importedImage);

            PrimaryType.Write(texture, xnbWriter);
        }
    }
}
