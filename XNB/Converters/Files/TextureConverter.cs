using FEZRepacker.XNB.Types.XNA;
using ImageMagick;
using ImageMagick.Formats;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace FEZRepacker.XNB.Converters.Files
{
    class TextureConverter : XNBContentConverter
    {
        public override XNBContentType[] Types => new XNBContentType[]
        {
            new Texture2DContentType(this)
        };
        public override string FileFormat => "png";

        public static MagickImage MagickImageFromTexture2D(Texture2D txt)
        {
            // FEZ textures shouldn't have additional mipmaps
            if (txt.MipmapLevels != 1)
            {
                throw new InvalidDataException($"Texture2D has unexpected mipmap levels count ({txt.MipmapLevels}).");
            }


            MagickReadSettings mr = new MagickReadSettings();
            mr.Width = txt.Width;
            mr.Height = txt.Height;
            mr.Format = MagickFormat.Rgba;

            // most of FEZ textures are saved in raw format
            // some of them are not. try to convert them.
            if (txt.Format != SurfaceFormat.Color)
            {
                if (txt.Format == SurfaceFormat.Dxt1 || txt.Format == SurfaceFormat.Dxt5)
                {
                    // Magick can convert DXT1 and DXT5 compressed textures, but it requires DDS header in the data.
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

                    txt.TextureData = newData;
                    mr.Format = MagickFormat.Dxt1;
                }
                else
                {
                    throw new InvalidDataException($"Texture2D has unsupported format ({txt.Format})");
                }
                txt.Format = SurfaceFormat.Color;
            }

            var image = new MagickImage(txt.TextureData, mr);
            return image;
        }


        public override void FromBinary(BinaryReader xnbReader, BinaryWriter outWriter)
        {
            Texture2D texture = (Texture2D)Types[0].Read(xnbReader);

            using var image = MagickImageFromTexture2D(texture);

            image.Write(outWriter.BaseStream, MagickFormat.Png);
        }

        public override void ToBinary(BinaryReader inReader, BinaryWriter xnbWriter)
        {
            MagickReadSettings mr = new MagickReadSettings();
            mr.Format = MagickFormat.Png;

            var importedImage = new MagickImage(inReader.BaseStream, mr);

            Texture2D texture = new Texture2D();
            texture.Format = SurfaceFormat.Color;
            texture.MipmapLevels = 1;
            texture.Width = importedImage.Width;
            texture.Height = importedImage.Height;
            texture.TextureData = importedImage.ToByteArray(MagickFormat.Rgba);

            PrimaryType.Write(texture, xnbWriter);
        }
    }
}
