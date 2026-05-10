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
            using var animation = Image.Load<Rgba32>(bundle.RequireData(""));
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
            var frames = ExtractFrameDataFromGif(animation);
            var animatedTexture = new AnimatedTexture()
            {
                FrameWidth = animation.Width,
                FrameHeight = animation.Height,
                Frames = frames,
            };
            animatedTexture.PackFrames(1);

            using var atlasImage = new Image<Rgba32>(animatedTexture.AtlasWidth, animatedTexture.AtlasHeight, Color.Transparent);
            PopulateAtlasImageFromGif(atlasImage, animation, frames);
            animatedTexture.TextureData = TexturesUtil.ImageToTexture2D(atlasImage).TextureData;

            return animatedTexture;
        }

        private static List<FrameContent> ExtractFrameDataFromGif(Image<Rgba32> animation)
        {
            var frames = new List<FrameContent>();

            foreach (ImageFrame<Rgba32> frameImg in animation.Frames)
            {
                frames.Add(new()
                {
                    Duration = TimeSpan.FromMilliseconds(frameImg.Metadata.GetGifMetadata().FrameDelay * 10),
                    Rectangle = new(0, 0, frameImg.Width, frameImg.Height)
                });
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
    }
}
