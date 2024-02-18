using System.Text;

using FEZRepacker.Core.Definitions.Game.ArtObject;
using FEZRepacker.Core.Definitions.Game.Graphics;
using FEZRepacker.Core.Definitions.Game.TrileSet;
using FEZRepacker.Core.Definitions.Game.XNA;
using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.Helpers;
using FEZRepacker.Core.Helpers.Json;

using SixLabors.ImageSharp.Formats.Png;

namespace FEZRepacker.Core.Conversion.Formats
{
    internal class TrileSetConverter : FormatConverter<TrileSet>
    {
        public override string FileFormat => ".fezts";

        public override FileBundle ConvertTyped(TrileSet data)
        {
            var bundle = ConfiguredJsonSerializer.SerializeToFileBundle(FileFormat, data);

            bundle.AddFile(GetTextureStream(data, TexturesUtil.CubemapPart.Albedo), ".png");
            bundle.AddFile(GetTextureStream(data, TexturesUtil.CubemapPart.Emission), ".apng");
            bundle.AddFile(GetModelStream(data), ".obj");

            return bundle;
        }

        public override TrileSet DeconvertTyped(FileBundle bundle)
        {
            var trileSet = ConfiguredJsonSerializer.DeserializeFromFileBundle<TrileSet>(bundle);

            AppendGeometryStream(ref trileSet, bundle.RequireData(".obj"));
            LoadCubemap(ref trileSet, bundle.GetData(".png"), bundle.GetData(".apng"));

            return trileSet;
        }

        private static Stream GetTextureStream(TrileSet data, TexturesUtil.CubemapPart part)
        {
            using var texture = TexturesUtil.ExtractCubemapPartFromTexture(data.TextureAtlas, part);
            return texture.SaveAsMemoryStream(new PngEncoder());
        }

        private static Stream GetModelStream(TrileSet data)
        {
            var geometryDict = new Dictionary<string, IndexedPrimitives<VertexInstance, Vector4>>();

            foreach (var trileRecord in data.Triles)
            {
                geometryDict[trileRecord.Key.ToString()] = trileRecord.Value.Geometry;
            }
            var objString = WavefrontObjUtil.ToWavefrontObj(geometryDict);
            return new MemoryStream(Encoding.UTF8.GetBytes(objString));
        }


        private static void AppendGeometryStream(ref TrileSet data, Stream geometryStream)
        {
            var geometries = TrixelArtUtil.LoadGeometry<Vector4>(geometryStream);
            foreach (var objRecord in geometries)
            {
                var id = int.Parse(objRecord.Key);

                if (!data.Triles.ContainsKey(id))
                {
                    data.Triles[id] = new Trile();
                }
                data.Triles[id].Geometry = objRecord.Value;
            }
        }

        private static void LoadCubemap(ref TrileSet data, Stream albedoStream, Stream emissionStream)
        {
            using var image = TexturesUtil.ConstructCubemap(albedoStream, emissionStream);
            data.TextureAtlas = TexturesUtil.ImageToTexture2D(image);
        }
    }
}
