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
            var texture = SaveCubemap(trileSet);
            var data = SaveAdditionalData(trileSet);

            return new FileBundle(FileFormat)
            {
                (".obj", geometry),
                (".png", texture),
                (".json", data)
            };
        }

        public override void WriteXnbContent(FileBundle bundle, BinaryWriter xnbWriter)
        {
            TrileSet trileSet = new TrileSet();

            foreach (var file in bundle)
            {
                if (file.Extension == ".obj") LoadGeometry(file.Data, ref trileSet);
                if (file.Extension == ".png") LoadCubemap(file.Data, ref trileSet);
                if (file.Extension == ".json") LoadAdditionalData(file.Data, ref trileSet);
            }

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

        private static Stream SaveCubemap(TrileSet trileSet)
        {
            var memoryStream = new MemoryStream();

            if (trileSet.TextureAtlas.TextureData.Length == 0)
            {
                return memoryStream;
            }

            var image = TextureConverter.ImageFromTexture2D(trileSet.TextureAtlas);

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

                if (!trileSet.Triles.ContainsKey(id)) trileSet.Triles[id] = new Trile();
                trileSet.Triles[id].Geometry = objRecord.Value;
            }
        }

        private static void LoadCubemap(Stream imageStream, ref TrileSet trileSet)
        {
            using var importedImage = Image.Load<Rgba32>(imageStream);
            trileSet.TextureAtlas = TextureConverter.ImageToTexture2D(importedImage);
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
                    trileSet.Triles[trileRecord.Key] = trileRecord.Value;
                }
            }
        }
    }
}
