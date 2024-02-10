using FEZRepacker.Core.Definitions.Game.Graphics;
using FEZRepacker.Core.Definitions.Game.XNA;
using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.Helpers;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using Color = SixLabors.ImageSharp.Color;

namespace FEZRepacker.Core.Conversion.Formats
{
    internal class AnimatedTextureConverter : FormatConverter<AnimatedTexture>
    {
        const int FramePadding = 1;

        public override string FileFormat => ".gif";

        public override FileBundle ConvertTyped(AnimatedTexture txt)
        {
            using var animation = AnimatedTextureToGif(txt);

            var outStream = animation.SaveAsMemoryStream(
                new GifEncoder { ColorTableMode = GifColorTableMode.Local }
            );

            return FileBundle.Single(outStream, FileFormat);
        }

        public override AnimatedTexture DeconvertTyped(FileBundle bundle)
        {
            using var animation = Image.Load<Rgba32>(bundle.GetData(""));
            return AnimationImageToAnimatedTexture(animation);
        }


        private static Image<Rgba32> AnimatedTextureToGif(AnimatedTexture txt)
        {
            using var atlasImage = AnimationTextureToAtlasImage(txt);

            var animation = new Image<Rgba32>(txt.FrameWidth, txt.FrameHeight, Color.Transparent);
            var animationMetadata = animation.Metadata.GetGifMetadata();
            animationMetadata.ColorTableMode = GifColorTableMode.Local;
            animationMetadata.RepeatCount = 0;

            for (int i = 0; i < txt.Frames.Count; i++)
            {
                var frame = txt.Frames[i];
                var rect = frame.Rectangle;

                using var frameImg = atlasImage.Clone(i =>
                    i.Crop(new(rect.X, rect.Y, rect.Width, rect.Height))
                );

                var frameMetadata = frameImg.Frames.RootFrame.Metadata.GetGifMetadata();
                frameMetadata.FrameDelay = frame.Duration.Milliseconds / 10;
                frameMetadata.DisposalMethod = GifDisposalMethod.RestoreToBackground;

                animation.Frames.AddFrame(frameImg.Frames.RootFrame);
            }

            // First blank frame is made when creating an image. Remove it.
            animation.Frames.RemoveFrame(0);

            return animation;
        }

        private static Image<Rgba32> AnimationTextureToAtlasImage(AnimatedTexture txt)
        {
            Texture2D atlas = new Texture2D()
            {
                Format = SurfaceFormat.Color,
                MipmapLevels = 1,
                Width = txt.AtlasWidth,
                Height = txt.AtlasHeight,
                TextureData = txt.TextureData
            };

            return TexturesUtil.ImageFromTexture2D(atlas);
        }

        private static AnimatedTexture AnimationImageToAnimatedTexture(Image<Rgba32> animation)
        {
            (var atlasWidth, var atlasHeight) = FindMinimumPowerOfTwoAtlasSize(animation);
            var frames = ExtractFrameDataFromGif(animation, atlasWidth);

            using var atlasImage = new Image<Rgba32>(atlasWidth, atlasHeight, Color.Transparent);
            PopulateAtlasImageFromGif(atlasImage, animation, frames);
            var atlas = TexturesUtil.ImageToTexture2D(atlasImage);

            return new AnimatedTexture()
            {
                AtlasWidth = atlasWidth,
                AtlasHeight = atlasHeight,
                FrameWidth = animation.Width,
                FrameHeight = animation.Height,

                Frames = frames,
                TextureData = atlas.TextureData,
            };
        }

        private static List<FrameContent> ExtractFrameDataFromGif(Image<Rgba32> animation, int atlasWidth)
        {
            var frames = new List<FrameContent>();

            int framePosX = 0;
            int framePosY = 0;
            foreach (ImageFrame<Rgba32> frameImg in animation.Frames)
            {
                frames.Add(new()
                {
                    Duration = TimeSpan.FromMilliseconds(frameImg.Metadata.GetGifMetadata().FrameDelay * 10),
                    Rectangle = new(framePosX, framePosY, frameImg.Width, frameImg.Height)
                });

                framePosX += animation.Width + FramePadding;
                if (framePosX > atlasWidth - animation.Width)
                {
                    framePosX = 0;
                    framePosY += animation.Height + FramePadding;
                }
            }

            return frames;
        }

        private static void PopulateAtlasImageFromGif(Image<Rgba32> atlasImage, Image<Rgba32> animation, List<FrameContent> framesData)
        {
            for (int i = 0; i < framesData.Count; i++)
            {
                var frameImg = animation.Frames[i];
                var frameRect = framesData[i].Rectangle;
                frameImg.ProcessPixelRows(atlasImage.Frames.RootFrame, (animAccessor, atlasAccessor) =>
                {
                    for (int y = 0; y < frameImg.Height; y++)
                    {
                        animAccessor.GetRowSpan(y).CopyTo(atlasAccessor.GetRowSpan(frameRect.Y + y).Slice(frameRect.X));
                    }
                });
            }
        }


        // Calculating the minimum size of the atlas for the animation
        // with both width and height being powers of two
        private static (int Width, int Height) FindMinimumPowerOfTwoAtlasSize(Image animatedImage)
        {
            int atlasWidth = 0;
            int atlasHeight = 0;
            int atlasArea = int.MaxValue;

            for (int i = 1; i <= animatedImage.Frames.Count; i++)
            {
                var framesInRow = i;
                var framesInColumn = (int)Math.Ceiling(animatedImage.Frames.Count / (float)i);

                int newAtlasWidth = (animatedImage.Width + FramePadding) * framesInRow;
                int newAtlasHeight = (animatedImage.Height + FramePadding) * framesInColumn;

                newAtlasWidth = (int)Math.Pow(2, Math.Ceiling(Math.Log(newAtlasWidth, 2)));
                newAtlasHeight = (int)Math.Pow(2, Math.Ceiling(Math.Log(newAtlasHeight, 2)));

                if (newAtlasWidth > newAtlasHeight && atlasWidth > 0 && atlasHeight > 0) break;

                int newArea = newAtlasWidth * newAtlasHeight;
                if (newArea <= atlasArea)
                {
                    atlasArea = newArea;
                    atlasWidth = newAtlasWidth;
                    atlasHeight = newAtlasHeight;
                }
            }

            return (atlasWidth, atlasHeight);
        }
    }
}
