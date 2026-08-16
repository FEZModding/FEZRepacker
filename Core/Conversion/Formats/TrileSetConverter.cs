using System.Text;
using System.Text.Json.Nodes;

using FEZRepacker.Core.Definitions.Game.ArtObject;
using FEZRepacker.Core.Definitions.Game.Graphics;
using FEZRepacker.Core.Definitions.Game.TrileSet;
using FEZRepacker.Core.Definitions.Game.XNA;
using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.Helpers;
using FEZRepacker.Core.Helpers.Json;

using SharpGLTF.Schema2;

using SixLabors.ImageSharp.Formats.Png;

namespace FEZRepacker.Core.Conversion.Formats
{
    internal class TrileSetConverter : FormatConverter<TrileSet>
    {
        private const string BundleFileFormat = ".fezts";
        private const string TrileIdKey = "TrileId";
        
        public override string[] FileFormats => [BundleFileFormat];


        public override FileBundle ConvertTyped(TrileSet data)
        {
            if (!Settings.UseTrixelArtBundle)
            {
                return FileBundle.Single(GetTransmissionFormatStream(data), BundleFileFormat, ".glb");
            }
            
            var bundle = ConfiguredJsonSerializer.SerializeToFileBundle(BundleFileFormat, data);

            if (data.TextureAtlas is { } textureAtlas)
            {
                bundle.AddFile(GetTextureStream(textureAtlas, TexturesUtil.CubemapPart.Albedo), ".png");
                bundle.AddFile(GetTextureStream(textureAtlas, TexturesUtil.CubemapPart.Emission), ".apng");
            }

            bundle.AddFile(GetModelStream(data), ".obj");

            return bundle;
        }

        public override TrileSet DeconvertTyped(FileBundle bundle)
        {
            try
            {
                return LoadFromTransmissionFormat(bundle.RequireData(".glb"));
            }
            catch (FileNotFoundException)
            {
                var trileSet = ConfiguredJsonSerializer.DeserializeFromFileBundle<TrileSet>(bundle);
                AppendGeometryStream(ref trileSet, bundle.RequireData(".obj"));
                LoadCubemap(ref trileSet, bundle.GetData(".png"), bundle.GetData(".apng"));
            
                return trileSet;
            }
        }

        private static Stream GetTextureStream(Texture2D textureAtlas, TexturesUtil.CubemapPart part)
        {
            using var texture = TexturesUtil.ExtractCubemapPartFromTexture(textureAtlas, part);
            return texture.SaveAsMemoryStream(new PngEncoder());
        }

        private static Stream GetModelStream(TrileSet data)
        {
            var geometryDict = new Dictionary<string, IndexedPrimitives<VertexInstance, Vector4>>();

            foreach (var trileRecord in data.Triles)
            {
                if (trileRecord.Value.Geometry is not { } geometry) continue;
                geometryDict[trileRecord.Key.ToString()] = geometry.WithReversedWindingIndices();
            }

            var objString = WavefrontObjUtil.ToWavefrontObj(geometryDict);
            return new MemoryStream(Encoding.UTF8.GetBytes(objString));
        }

        private static Stream GetTransmissionFormatStream(TrileSet data)
        {
            var entries = new List<GltfEntry<Vector4>>();
            foreach (var trileRecord in data.Triles)
            {
                var extras = ConfiguredJsonSerializer.SerializeToNode(trileRecord.Value) ?? new JsonObject();
                extras[TrileIdKey] = trileRecord.Key;
                var geometry = trileRecord.Value.Geometry?.WithReversedWindingIndices();
                entries.Add(new GltfEntry<Vector4>(trileRecord.Value.Name, geometry, extras));
            }

            using var albedo = data.TextureAtlas is { } albedoAtlas
                ? TexturesUtil.ExtractCubemapPartFromTexture(albedoAtlas, TexturesUtil.CubemapPart.Albedo)
                : null;
            using var emission = data.TextureAtlas is { } emissionAtlas
                ? TexturesUtil.ExtractCubemapPartFromTexture(emissionAtlas, TexturesUtil.CubemapPart.Emission)
                : null;

            return GltfUtil.ToGltfModel(data.Name, entries, albedo, emission).SaveAsGlb();
        }
        
        private static TrileSet LoadFromTransmissionFormat(Stream modelStream)
        {
            var modelRoot = ModelRoot.ReadGLB(modelStream);
            var trileSet = new TrileSet { Name = modelRoot.DefaultScene.Name };

            var entries = GltfUtil.FromGltfModel<Vector4>(modelRoot);
            foreach (var entry in entries)
            {
                if (entry.Extras?[TrileIdKey] == null)
                {
                    continue;
                }
                
                var id = entry.Extras[TrileIdKey]!.GetValue<int>();
                if (!trileSet.Triles.ContainsKey(id))
                {
                    trileSet.Triles[id] = ConfiguredJsonSerializer.DeserializeFromNode<Trile>(entry.Extras) ?? new Trile();
                }

                trileSet.Triles[id].Geometry = entry.Geometry?.WithReversedWindingIndices();
            }

            (Stream? albedo, Stream? emission) = GltfUtil.ExtractCubemapStreams(modelRoot);
            LoadCubemap(ref trileSet, albedo, emission);
            
            return trileSet;
        }

        private static void AppendGeometryStream(ref TrileSet data, Stream geometryStream)
        {
            var geometries = WavefrontObjUtil.FromWavefrontObjStream<Vector4>(geometryStream);
            foreach (var objRecord in geometries)
            {
                // Groups not named after a trile id hold no trile geometry.
                if (!int.TryParse(objRecord.Key, out var id)) continue;

                if (!data.Triles.ContainsKey(id))
                {
                    data.Triles[id] = new Trile();
                }

                data.Triles[id].Geometry = objRecord.Value.WithReversedWindingIndices();
            }
        }

        private static void LoadCubemap(ref TrileSet data, Stream? albedoStream, Stream? emissionStream)
        {
            using var image = TexturesUtil.ConstructCubemap(albedoStream, emissionStream);
            data.TextureAtlas = image == null ? null : TexturesUtil.ImageToTexture2D(image);
        }
    }
}