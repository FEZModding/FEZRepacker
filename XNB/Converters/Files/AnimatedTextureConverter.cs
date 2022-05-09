using FezEngine.Structure;
using FEZRepacker.XNB.Types.System;
using FEZRepacker.XNB.Types.XNA;
using ImageMagick;
using ImageMagick.Formats;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace FEZRepacker.XNB.Converters.Files
{
    class AnimatedTextureConverter : XNBContentConverter
    {
        public override XNBContentType[] Types => new XNBContentType[]
        {
            new AnimatedTextureContentType(this),
            new ListContentType<FrameContent>(this),
            new FrameContentContentType(this),
            new TimeSpanContentType(this),
            new RectangleContentReader(this)
        };
        public override string FileFormat => "webp";

        public override void FromBinary(BinaryReader xnbReader, BinaryWriter outWriter)
        {
            AnimatedTexture txt = (AnimatedTexture)Types[0].Read(xnbReader);

            using var image = TextureConverter.MagickImageFromTexture2D(txt.Texture);

            using var animation = new MagickImageCollection();
            
            foreach(var frame in txt.Frames)
            {
                var frameImg = image.Clone();
                var rect = frame.Rectangle;
                frameImg.Crop(new MagickGeometry(rect.X, rect.Y, rect.Width, rect.Height));
                frameImg.RePage();
                frameImg.AnimationDelay = frame.Duration.Milliseconds / 10;
                frameImg.GifDisposeMethod = GifDisposeMethod.Previous;
                animation.Add(frameImg);
            }

            animation.Write(outWriter.BaseStream, new WebPWriteDefines
            {
                Lossless = true,
                Method = 6,
            });
        }

        public override void ToBinary(BinaryReader inReader, BinaryWriter xnbWriter)
        {

        }
    }
}
