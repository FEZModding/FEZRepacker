using FEZRepacker.XNB.Types.XNA;
using ImageMagick;
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

        public override void FromBinary(BinaryReader xnbReader, BinaryWriter outWriter)
        {
            Texture2D texture = (Texture2D)Types[0].Read(xnbReader);

            MagickReadSettings mr = new MagickReadSettings();
            if (texture.Format == SurfaceFormat.Dxt1) mr.Compression = CompressionMethod.DXT1;
            mr.Format = MagickFormat.Rgba;
            mr.Width = texture.Width;
            mr.Height = texture.Height;
            var image = new MagickImage(texture.TextureData, mr);

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
