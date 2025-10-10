using System.Text;
using System.Text.Json.Nodes;

using FEZRepacker.Core.Definitions.Game.ArtObject;
using FEZRepacker.Core.Definitions.Game.Graphics;
using FEZRepacker.Core.Definitions.Game.XNA;
using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.Helpers;
using FEZRepacker.Core.Helpers.Json;

using SharpGLTF.Schema2;
using SharpGLTF.Validation;

using SixLabors.ImageSharp.Formats.Png;

namespace FEZRepacker.Core.Conversion.Formats
{
    internal class ArtObjectConverter : FormatConverter<ArtObject>
    {
        public override string FileFormat => ".fezao";

        public override FileBundle ConvertTyped(ArtObject data)
        {
            if (!Settings.UseLegacyArtObjectBundle)
            {
                return FileBundle.Single(GetTransmissionFormatStream(data), FileFormat, ".glb");
            }
            
            var bundle = ConfiguredJsonSerializer.SerializeToFileBundle(FileFormat, data);

            bundle.AddFile(GetTextureStream(data, TexturesUtil.CubemapPart.Albedo), ".png");
            bundle.AddFile(GetTextureStream(data, TexturesUtil.CubemapPart.Emission), ".apng");
            bundle.AddFile(GetModelStream(data), ".obj");

            return bundle;
        }

        public override ArtObject DeconvertTyped(FileBundle bundle)
        {
            try
            {
                return LoadFromTransmissionFormat(bundle.RequireData(".glb"));
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("  The glTF bundle was not found! Using legacy art object bundle format...");
                var artObject = ConfiguredJsonSerializer.DeserializeFromFileBundle<ArtObject>(bundle);

                AppendGeometryStream(ref artObject, bundle.RequireData(".obj"));
                LoadCubemap(ref artObject, bundle.GetData(".png"), bundle.GetData(".apng"));

                return artObject;
            }
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

        private static Stream GetTransmissionFormatStream(ArtObject data)
        {
            using var albedo =
                TexturesUtil.ExtractCubemapPartFromTexture(data.Cubemap, TexturesUtil.CubemapPart.Albedo);
            using var emission =
                TexturesUtil.ExtractCubemapPartFromTexture(data.Cubemap, TexturesUtil.CubemapPart.Emission);
            var extras = ConfiguredJsonSerializer.SerializeToNode(data);
            var entry = new GltfEntry<Matrix>(data.Name, data.Geometry, extras);
            return GltfUtil.ToGltfModel(entry, albedo, emission).SaveAsGlb();
        }

        private static ArtObject LoadFromTransmissionFormat(Stream modelStream)
        {
            var modelRoot = ModelRoot.ReadGLB(modelStream);
            var entries = GltfUtil.FromGltfModel<Matrix>(modelRoot);
            
            if (entries.Count < 1)
                return new ArtObject();

            var entry = entries.First();
            var artObject = ConfiguredJsonSerializer.DeserializeFromNode<ArtObject>(entry.Extras) ?? new ArtObject();
            artObject.Geometry = entry.Geometry;
            TrixelArtUtil.RecalculateCubemapTexCoords(artObject.Geometry, artObject.Size);
            
            (Stream? albedo, Stream? emission) = GltfUtil.ExtractCubemapStreams(modelRoot);
            LoadCubemap(ref artObject, albedo, emission);

            return artObject;
        }

        private static void AppendGeometryStream(ref ArtObject data, Stream geometryStream)
        {
            var geometries = TrixelArtUtil.LoadGeometry<Matrix>(geometryStream);
            if (geometries.Count < 1) return;
            data.Geometry = geometries.First().Value;
            TrixelArtUtil.RecalculateCubemapTexCoords(data.Geometry, data.Size);
        }

        private static void LoadCubemap(ref ArtObject data, Stream? albedoStream, Stream? emissionStream)
        {
            using var image = TexturesUtil.ConstructCubemap(albedoStream, emissionStream);
            data.Cubemap = TexturesUtil.ImageToTexture2D(image);
        }
    }
}