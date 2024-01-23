using System.Text;

using FEZRepacker.Core.Definitions.Game.ArtObject;
using FEZRepacker.Core.Definitions.Game.XNA;
using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.Helpers;
using FEZRepacker.Core.Helpers.Json;

using SixLabors.ImageSharp.Formats.Png;

namespace FEZRepacker.Core.Conversion
{
    internal class ArtObjectConverter : FormatConverter<ArtObject>
    {
        public override string FileFormat => ".fezao";

        public override FileBundle ConvertTyped(ArtObject data)
        {
            var bundle = ConfiguredJsonSerializer.SerializeToFileBundle(FileFormat, data);

            bundle.AddFile(GetTextureStream(data, TexturesUtil.CubemapPart.Albedo), ".png");
            bundle.AddFile(GetTextureStream(data, TexturesUtil.CubemapPart.Emission), ".apng");
            bundle.AddFile(GetModelStream(data), ".obj");

            return bundle;
        }

        public override ArtObject DeconvertTyped(FileBundle bundle)
        {
            var artObject = ConfiguredJsonSerializer.DeserializeFromFileBundle<ArtObject>(bundle);

            AppendGeometryStream(ref artObject, bundle.GetData(".obj"));
            LoadCubemap(ref artObject, bundle.GetData(".png"), bundle.GetData(".apng"));


            return artObject;
        }

        private static Stream GetTextureStream(ArtObject data, TexturesUtil.CubemapPart part)
        {
            using var texture = TexturesUtil.ExtractCubemapPartFromTexture(data.Cubemap, part);
            return texture.SaveAsMemoryStream(new PngEncoder());
        }

        private static Stream GetModelStream(ArtObject data)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(data.Geometry.ToWavefrontObj()));
        }

        private static void AppendGeometryStream(ref ArtObject data, Stream geometryStream)
        {
            var geometries = TrixelArtUtil.LoadGeometry<Matrix>(geometryStream);
            if (geometries.Count() > 0)
            {
                data.Geometry = geometries.First().Value;
                TrixelArtUtil.RecalculateCubemapTexCoords(data.Geometry, data.Size);
            }
        }

        private static void LoadCubemap(ref ArtObject data, Stream albedoStream, Stream emissionStream)
        {
            using var image = TexturesUtil.ConstructCubemap(albedoStream, emissionStream);
            data.Cubemap = TexturesUtil.ImageToTexture2D(image);
        }
    }
}
