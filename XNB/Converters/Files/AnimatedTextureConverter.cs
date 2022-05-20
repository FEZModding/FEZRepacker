using FezEngine.Content;
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

            for (int i = 0; i < txt.Frames.Count; i++)
            {
                var frame = txt.Frames[i];
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
                Method = 0,
            });
        }

        public override void ToBinary(BinaryReader inReader, BinaryWriter xnbWriter)
        {
            AnimatedTexture animatedTexture = new AnimatedTexture();

            MagickReadSettings mr = new MagickReadSettings();
            mr.Format = MagickFormat.WebP;

            using var importedAnim = new MagickImageCollection(inReader.BaseStream, mr);
            importedAnim.Coalesce();

            // calculate texture atlas size (least size when using powers of two)
            // why powers of two you may ask? idk, original textures seem to do that,
            // gpu like powers of two, so why not
            
            int frameWidth = importedAnim[0].Width;
            int frameHeight = importedAnim[0].Height;
            int frameCount = importedAnim.Count;

            int atlasWidth = 0;
            int atlasHeight = 0;
            int atlasArea = Int32.MaxValue; 

            for(int i=1; i <= frameCount; i++)
            {
                //calculating minimum size
                int newAtlasWidth = (frameWidth+1) * i;
                int newAtlasHeight = (frameHeight+1) * (int)(Math.Ceiling(frameCount / (float)i));

                // rounding the size up to nearest power of two
                newAtlasWidth = (int)Math.Pow(2, Math.Ceiling(Math.Log2(newAtlasWidth)));
                newAtlasHeight = (int)Math.Pow(2, Math.Ceiling(Math.Log2(newAtlasHeight)));

                if (newAtlasWidth > newAtlasHeight) break;

                int newArea = newAtlasWidth * newAtlasHeight;
                if(newArea <= atlasArea)
                {
                    atlasArea = newArea;
                    atlasWidth = newAtlasWidth;
                    atlasHeight = newAtlasHeight;
                }
            }

            // constructing the animated texture along with atlas
            using MagickImage atlasImage = new MagickImage(new MagickColor(0,0,0,0), atlasWidth, atlasHeight);

            int framePosX = 0;
            int framePosY = 0;
            for(int i=0; i< frameCount; i++)
            {
                atlasImage.Composite(importedAnim[i], framePosX, framePosY, CompositeOperator.Copy);

                FrameContent frame = new FrameContent();
                frame.Duration = TimeSpan.FromMilliseconds(importedAnim[i].AnimationDelay * 10);
                frame.Rectangle = new Rectangle(framePosX, framePosY, frameWidth, frameHeight);
                animatedTexture.Frames.Add(frame);

                framePosX += frameWidth + 1; // pixel padding between sprites
                if(framePosX > atlasWidth - frameWidth)
                {
                    framePosX = 0;
                    framePosY += frameHeight + 1;
                }
            }

            animatedTexture.Texture = TextureConverter.MagickImageToTexture2D(atlasImage);
            animatedTexture.FrameWidth = frameWidth;
            animatedTexture.FrameHeight = frameHeight;

            PrimaryType.Write(animatedTexture, xnbWriter);
        }
    }
}
