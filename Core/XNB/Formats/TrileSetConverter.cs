using System.Numerics;
using System.Text;

using FEZRepacker.Converter.Definitions.FezEngine;
using FEZRepacker.Converter.Definitions.FezEngine.Structure;
using FEZRepacker.Converter.Definitions.FezEngine.Structure.Geometry;
using FEZRepacker.Converter.Definitions.MicrosoftXna;
using FEZRepacker.Converter.FileSystem;
using FEZRepacker.Converter.Helpers;
using FEZRepacker.Converter.XNB.Formats.Json;
using FEZRepacker.Converter.XNB.Formats.Json.CustomStructures;
using FEZRepacker.Converter.XNB.Types;
using FEZRepacker.Converter.XNB.Types.System;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

using Vector4 = FEZRepacker.Converter.Definitions.MicrosoftXna.Vector4;

namespace FEZRepacker.Converter.XNB.Formats
{
    internal class TrileSetConverter : XnbFormatConverter
    {
        public override List<XnbContentType> TypesFactory => new List<XnbContentType>
        {
            new GenericContentType<TrileSet>(this),
            new DictionaryContentType<int, Trile>(this),
            new Int32ContentType(this),
            new GenericContentType<Trile>(this),
            new DictionaryContentType<FaceOrientation, CollisionType>(this, true, true),
            new EnumContentType<FaceOrientation>(this),
            new EnumContentType<CollisionType>(this),
            new GenericContentType<ShaderInstancedIndexedPrimitives<VertexPositionNormalTextureInstance, Vector4>>(this),
            new GenericContentType<VertexPositionNormalTextureInstance>(this),
            new GenericContentType<Vector4>(this),
            new EnumContentType<PrimitiveType>(this),
            new ArrayContentType<VertexPositionNormalTextureInstance>(this),
            new ArrayContentType<ushort>(this),
            new UInt16ContentType(this),
            new EnumContentType<ActorType>(this),
            new EnumContentType<SurfaceType>(this),
            new GenericContentType<Texture2D>(this),
            new EnumContentType<SurfaceFormat>(this),
            new ByteArrayContentType(this)
        };
        public override string FileFormat => ".fezts";

        public override FileBundle ReadXNBContent(BinaryReader xnbReader)
        {
            TrileSet trileSet = (TrileSet)PrimaryContentType.Read(xnbReader);

            var geometry = SaveGeometry(trileSet);
            var albedoTexture = SaveCubemap(trileSet, false);
            var emissionTexture = SaveCubemap(trileSet, true);
            var data = SaveAdditionalData(trileSet);

            var bundle = new FileBundle(FileFormat);
            bundle.AddFile(geometry, ".obj");
            bundle.AddFile(albedoTexture, ".png");
            bundle.AddFile(emissionTexture, ".apng");
            bundle.AddFile(data, ".json");
            return bundle;
        }

        public override void WriteXnbContent(FileBundle bundle, BinaryWriter xnbWriter)
        {
            TrileSet trileSet = new TrileSet();

            Stream albedoData = null;
            Stream emissionData = null;

            foreach (var file in bundle.Files)
            {
                if (file.Extension == ".obj") LoadGeometry(file.Data, ref trileSet);
                if (file.Extension == ".png") albedoData = file.Data;
                if (file.Extension == ".apng") emissionData = file.Data;
                if (file.Extension == ".json") LoadAdditionalData(file.Data, ref trileSet);
            }

            LoadCubemap(albedoData, emissionData, ref trileSet);

            PrimaryContentType.Write(trileSet, xnbWriter);
        }



        private static Stream SaveGeometry(TrileSet trileSet)
        {
            var geometryDict = new Dictionary<string, ShaderInstancedIndexedPrimitives<VertexPositionNormalTextureInstance, Vector4>>();

            foreach(var trileRecord in trileSet.Triles)
            {
                geometryDict[trileRecord.Key.ToString()] = trileRecord.Value.Geometry;
            }
            var objString = WavefrontObjUtil.ToWavefrontObj(geometryDict);
            return new MemoryStream(Encoding.UTF8.GetBytes(objString));
        }

        private static Stream SaveCubemap(TrileSet trileSet, bool emission)
        {
            var memoryStream = new MemoryStream();

            if (trileSet.TextureAtlas.TextureData.Length == 0)
            {
                return memoryStream;
            }

            using var image = TextureConverter.ImageFromTexture2D(trileSet.TextureAtlas);

            // separate emission encoded into alpha channel from albedo
            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    var pixelRow = accessor.GetRowSpan(y);
                    foreach (ref Rgba32 pixel in pixelRow)
                    {
                        if (emission) pixel.R = pixel.G = pixel.B = pixel.A;
                        pixel.A = 255;
                    }
                }
            });

            image.Save(memoryStream, new PngEncoder());
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        private static Stream SaveAdditionalData(TrileSet trileSet)
        {
            var json = CustomJsonSerializer.Serialize(trileSet);
            return new MemoryStream(Encoding.UTF8.GetBytes(json));
        }

        private static void LoadGeometry(Stream geometryStream, ref TrileSet trileSet)
        {
            using var geometryReader = new BinaryReader(geometryStream, Encoding.UTF8, true);
            string geometryString = new string(geometryReader.ReadChars((int)geometryStream.Length));
            var geometry = WavefrontObjUtil.FromWavefrontObj<Vector4>(geometryString);
            foreach(var objRecord in geometry)
            {
                var id = int.Parse(objRecord.Key);

                if (!trileSet.Triles.ContainsKey(id))
                {
                    trileSet.Triles[id] = new Trile();
                }
                trileSet.Triles[id].Geometry = objRecord.Value;
            }
        }

        private static void LoadCubemap(Stream imageAlbedoStream, Stream imageEmissionStream, ref TrileSet ts)
        {
            if (imageAlbedoStream == null && imageEmissionStream == null) return;

            // use emission stream as albedo if albedo is not present
            using var albedoImage = Image.Load<Rgba32>(imageAlbedoStream ?? imageEmissionStream);

            using var emissionImage =
                imageEmissionStream != null
                ? Image.Load<Rgba32>(imageEmissionStream)
                : new Image<Rgba32>(albedoImage.Width, albedoImage.Height, Color.Black);

            albedoImage.ProcessPixelRows(emissionImage, (albedoAccesor, emissionAccesor) =>
            {
                for (int y = 0; y < albedoAccesor.Height; y++)
                {
                    var albedoPixelRow = albedoAccesor.GetRowSpan(y);
                    var emissionPixelRow = emissionAccesor.GetRowSpan(y);
                    for (int x = 0; x < albedoPixelRow.Length; x++)
                    {
                        albedoPixelRow[x].A = emissionPixelRow[x].R;
                    }
                }
            });

            ts.TextureAtlas = TextureConverter.ImageToTexture2D(albedoImage);
        }

        private static void LoadAdditionalData(Stream jsonStream, ref TrileSet trileSet)
        {
            using var jsonReader = new BinaryReader(jsonStream, Encoding.UTF8, true);
            string json = new string(jsonReader.ReadChars((int)jsonStream.Length));
            var data = CustomJsonSerializer.Deserialize<TrileSet>(json);

            trileSet.Name = data.Name;
            foreach(var trileRecord in data.Triles)
            {
                if (trileSet.Triles.ContainsKey(trileRecord.Key))
                {
                    trileRecord.Value.Geometry = trileSet.Triles[trileRecord.Key].Geometry;
                }
                trileSet.Triles[trileRecord.Key] = trileRecord.Value;
            }
        }
    }
}
