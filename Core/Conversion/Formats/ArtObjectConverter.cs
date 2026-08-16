using System.Text;

using FEZRepacker.Core.Definitions.Game.ArtObject;
using FEZRepacker.Core.Definitions.Game.Graphics;
using FEZRepacker.Core.Definitions.Game.XNA;
using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.Helpers;
using FEZRepacker.Core.Helpers.Json;

using SharpGLTF.Schema2;
using SixLabors.ImageSharp.Formats.Png;

namespace FEZRepacker.Core.Conversion.Formats
{
    internal class ArtObjectConverter : FormatConverter<ArtObject>
    {
        private const string BundleFileFormat = ".fezao";
        
        public override string[] FileFormats => [BundleFileFormat];

        public override FileBundle ConvertTyped(ArtObject data)
        {
            if (!Settings.UseTrixelArtBundle)
            {
                return FileBundle.Single(GetTransmissionFormatStream(data), BundleFileFormat, ".glb");
            }
            
            var bundle = ConfiguredJsonSerializer.SerializeToFileBundle(BundleFileFormat, data);

            if (data.Cubemap is { } cubemap)
            {
                bundle.AddFile(GetTextureStream(cubemap, TexturesUtil.CubemapPart.Albedo), ".png");
                bundle.AddFile(GetTextureStream(cubemap, TexturesUtil.CubemapPart.Emission), ".apng");
            }

            if (data.Geometry is { } geometry)
            {
                bundle.AddFile(GetModelStream(geometry), ".obj");
            }

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

                AppendGeometryStream(ref artObject, bundle.GetData(".obj"));
                LoadCubemap(ref artObject, bundle.GetData(".png"), bundle.GetData(".apng"));

                return artObject;
            }
        }

        private static Stream GetTextureStream(Texture2D cubemap, TexturesUtil.CubemapPart part)
        {
            using var texture = TexturesUtil.ExtractCubemapPartFromTexture(cubemap, part);
            return texture.SaveAsMemoryStream(new PngEncoder());
        }

        private static Stream GetModelStream(IndexedPrimitives<VertexInstance, Matrix> geometry)
        {
            var reversedGeometry = geometry.WithReversedWindingIndices();
            return new MemoryStream(Encoding.UTF8.GetBytes(reversedGeometry.ToWavefrontObj()));
        }

        private static Stream GetTransmissionFormatStream(ArtObject data)
        {
            using var albedo = data.Cubemap is { } albedoCubemap
                ? TexturesUtil.ExtractCubemapPartFromTexture(albedoCubemap, TexturesUtil.CubemapPart.Albedo)
                : null;
            using var emission = data.Cubemap is { } emissionCubemap
                ? TexturesUtil.ExtractCubemapPartFromTexture(emissionCubemap, TexturesUtil.CubemapPart.Emission)
                : null;

            var extras = ConfiguredJsonSerializer.SerializeToNode(data);
            var entry = new GltfEntry<Matrix>(data.Name, data.Geometry?.WithReversedWindingIndices(), extras);
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
            artObject.Geometry = entry.Geometry?.WithReversedWindingIndices();
            if (artObject.Geometry is { } geometry)
            {
                FezGeometryUtil.RecalculateCubemapTexCoords(geometry, artObject.Size, true);
            }


            (Stream? albedo, Stream? emission) = GltfUtil.ExtractCubemapStreams(modelRoot);
            LoadCubemap(ref artObject, albedo, emission);

            return artObject;
        }

        private static void AppendGeometryStream(ref ArtObject data, Stream? geometryStream)
        {
            if (geometryStream == null) return;

            var geometries = WavefrontObjUtil.FromWavefrontObjStream<Matrix>(geometryStream);
            if (geometries.Count < 1) return;

            var geometry = geometries.First().Value.WithReversedWindingIndices();
            FezGeometryUtil.RecalculateCubemapTexCoords(geometry, data.Size, true);
            data.Geometry = geometry;
        }

        private static void LoadCubemap(ref ArtObject data, Stream? albedoStream, Stream? emissionStream)
        {
            using var image = TexturesUtil.ConstructCubemap(albedoStream, emissionStream);
            data.Cubemap = image == null ? null : TexturesUtil.ImageToTexture2D(image);
        }
    }
}