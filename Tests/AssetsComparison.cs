using FEZRepacker.Core.Definitions.Game.ArtObject;
using FEZRepacker.Core.Definitions.Game.Graphics;
using FEZRepacker.Core.Definitions.Game.TrileSet;
using FEZRepacker.Core.Definitions.Game.XNA;
using FEZRepacker.Core.Helpers;
using FEZRepacker.Core.XNB;

namespace FEZRepacker.Tests
{
    internal static class AssetsComparison
    {
        public enum Result
        {
            Identical, // bitwise identical
            Equivalent, // acceptable differences
            Different // we screwed up
        }
        
        public static Result ConsumeAndCompare(object original, object roundTripped, out string difference)
        {
            difference = "";
            
            try
            {
                var originalXnb = SerializeAssetToXnbBytes(original);
                var roundTrippedXnb = SerializeAssetToXnbBytes(roundTripped);
                if (originalXnb.SequenceEqual(roundTrippedXnb))
                {
                    return Result.Identical;
                }

                if (original is AnimatedTexture originalAnimation && roundTripped is AnimatedTexture roundTrippedAnimation)
                {
                    difference = DescribeAnimationDifference(originalAnimation, roundTrippedAnimation);
                    if (difference.Length != 0)
                    {
                        return Result.Different;
                    }
                    return Result.Equivalent;
                }

                var expected = SerializeAssetToXnbBytes(WithGeometryFlattened(WithTexturesReencoded(original)));
                var actual = SerializeAssetToXnbBytes(WithGeometryFlattened(roundTripped));
                if (expected.SequenceEqual(actual))
                {
                    return Result.Equivalent;
                }

                difference = DescribeByteDifference(expected, actual);
                return Result.Different;
            }
            catch (Exception exception)
            {
                difference = $"{exception.GetType().Name}: {exception.Message}";
                return Result.Different;
            }
        }

        private static byte[] SerializeAssetToXnbBytes(object asset)
        {
            using var stream = XnbSerializer.Serialize(asset);
            using var buffer = new MemoryStream();
            stream.CopyTo(buffer);
            return buffer.ToArray();
        }

        private static object WithTexturesReencoded(object asset)
        {
            switch (asset)
            {
                case Texture2D texture:
                    return ReencodeTexture(texture);

                case SpriteFont font:
                    font.Texture = ReencodeTexture(font.Texture);
                    return font;

                case ArtObject artObject:
                    if (artObject.Cubemap != null)
                    {
                        artObject.Cubemap = ReencodeTexture(artObject.Cubemap);
                    }
                    return artObject;

                case TrileSet trileSet:
                    if (trileSet.TextureAtlas != null)
                    {
                        trileSet.TextureAtlas = ReencodeTexture(trileSet.TextureAtlas);
                    }
                    return trileSet;

                default:
                    return asset;
            }
        }
        
        private static Texture2D ReencodeTexture(Texture2D texture)
        {
            var topLevel = new Texture2D
            {
                Format = texture.Format,
                Width = texture.Width,
                Height = texture.Height,
                MipmapLevels = 1,
                TextureData = texture.TextureData.Take(TopMipmapLength(texture)).ToArray()
            };

            using var image = TexturesUtil.ImageFromTexture2D(topLevel);
            return TexturesUtil.ImageToTexture2D(image, texture.Format);
        }

        private static int TopMipmapLength(Texture2D texture)
        {
            var blockCount = ((texture.Width + 3) / 4) * ((texture.Height + 3) / 4);
            return texture.Format switch
            {
                SurfaceFormat.Dxt1 => blockCount * 8,
                SurfaceFormat.Dxt3 or SurfaceFormat.Dxt5 => blockCount * 16,
                _ => texture.Width * texture.Height * 4
            };
        }
        
        private static object WithGeometryFlattened(object asset)
        {
            switch (asset)
            {
                case ArtObject artObject:
                    artObject.Geometry = FlattenGeometry(artObject.Geometry);
                    return artObject;

                case TrileSet trileSet:
                    foreach (var trile in trileSet.Triles.Values)
                    {
                        trile.Geometry = FlattenGeometry(trile.Geometry);
                    }
                    return trileSet;

                default:
                    return asset;
            }
        }
        
        private static IndexedPrimitives<VertexInstance, TInstanceType>? FlattenGeometry<TInstanceType>(
            IndexedPrimitives<VertexInstance, TInstanceType>? geometry)
        {
            if (geometry == null) return null;

            return new IndexedPrimitives<VertexInstance, TInstanceType>
            {
                PrimitiveType = geometry.PrimitiveType,
                Vertices = geometry.Indices.Select(index => geometry.Vertices[index]).ToArray(),
                Indices = []
            };
        }


        private static string DescribeAnimationDifference(AnimatedTexture original, AnimatedTexture roundTripped)
        {
            if (original.Frames.Count != roundTripped.Frames.Count)
            {
                return $"frame count {original.Frames.Count} -> {roundTripped.Frames.Count}";
            }

            if (original.FrameWidth != roundTripped.FrameWidth
                || original.FrameHeight != roundTripped.FrameHeight)
            {
                return $"frame size {original.FrameWidth}x{original.FrameHeight}"
                    + $" => {roundTripped.FrameWidth}x{roundTripped.FrameHeight}";
            }

            for (var i = 0; i < original.Frames.Count; i++)
            {
                var originalFrame = original.Frames[i];
                var roundTrippedFrame = roundTripped.Frames[i];

                if (originalFrame.Rectangle.Width != roundTrippedFrame.Rectangle.Width
                    || originalFrame.Rectangle.Height != roundTrippedFrame.Rectangle.Height)
                {
                    return $"frame {i} size {originalFrame.Rectangle.Width}x{originalFrame.Rectangle.Height}"
                        + $" => {roundTrippedFrame.Rectangle.Width}x{roundTrippedFrame.Rectangle.Height}";
                }

                var drift = (originalFrame.Duration - roundTrippedFrame.Duration).Duration();
                var acceptableDrift = TimeSpan.FromMilliseconds(20);
                if (drift > acceptableDrift)
                {
                    return $"frame {i} duration {originalFrame.Duration} => {roundTrippedFrame.Duration}";
                }
                
                /* TODO: we probably could compare individual frames here with some error factor,
                   since GIFs are lossly converted, but honestly if we care about round-tripping,
                   we should just make sure animation sheets are converted correctly. */
            }

            return "";
        }

        private static string DescribeByteDifference(byte[] original, byte[] roundTripped)
        {
            var shared = Math.Min(original.Length, roundTripped.Length);
            for (var i = 0; i < shared; i++)
            {
                if (original[i] == roundTripped[i]) continue;
                return $"{original.Length} => {roundTripped.Length} bytes, first differing at {i}"
                    + $" ({original[i]} => {roundTripped[i]})";
            }

            return $"{original.Length} => {roundTripped.Length} bytes";
        }
    }
}
