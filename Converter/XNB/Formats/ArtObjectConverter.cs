using System.Numerics;
using System.Text;

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

namespace FEZRepacker.Converter.XNB.Formats
{
    internal class ArtObjectConverter : XnbFormatConverter
    {
        public override List<XnbContentType> TypesFactory => new List<XnbContentType>
        {
            new GenericContentType<ArtObject>(this),
            new GenericContentType<Texture2D>(this),
            new EnumContentType<SurfaceFormat>(this),
            new ByteArrayContentType(this),
            new GenericContentType<ShaderInstancedIndexedPrimitives<VertexPositionNormalTextureInstance, Matrix>>(this),
            new GenericContentType<VertexPositionNormalTextureInstance>(this),
            new GenericContentType<Matrix>(this),
            new EnumContentType<PrimitiveType>(this),
            new Int32ContentType(this),
            new ArrayContentType<VertexPositionNormalTextureInstance>(this),
            new ArrayContentType<ushort>(this),
            new UInt16ContentType(this),
            new EnumContentType<ActorType>(this)
        };
        public override string FileFormat => ".fezao";

        public override FileBundle ReadXNBContent(BinaryReader xnbReader)
        {
            ArtObject ao = (ArtObject)PrimaryContentType.Read(xnbReader);

            var geometry = SaveGeometry(ao);
            var texture = SaveCubemap(ao);
            var data = SaveAdditionalData(ao);

            return new FileBundle(FileFormat)
            {
                (".obj", geometry),
                (".png", texture),
                (".json", data)
            };
        }

        public override void WriteXnbContent(FileBundle bundle, BinaryWriter xnbWriter)
        {
            ArtObject ao = new ArtObject();

            foreach(var file in bundle)
            {
                if (file.Extension == ".obj") LoadGeometry(file.Data, ref ao);
                if (file.Extension == ".png") LoadCubemap(file.Data, ref ao);
                if (file.Extension == ".json") LoadAdditionalData(file.Data, ref ao);
            }

            PrimaryContentType.Write(ao, xnbWriter);
        }



        private static Stream SaveGeometry(ArtObject ao)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(ao.Geometry.ToWavefrontObj()));
        }

        private static Stream SaveCubemap(ArtObject ao)
        {
            var memoryStream = new MemoryStream();

            if (ao.Cubemap.TextureData.Length == 0)
            {
                return memoryStream;
            }
            
            var image = TextureConverter.ImageFromTexture2D(ao.Cubemap);

            // ArtObject's cubemap alpha channel is presumably unused and set to 0 - make it 255.
            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    var pixelRow = accessor.GetRowSpan(y);
                    foreach (ref Rgba32 pixel in pixelRow)
                    {
                        pixel.A = 255;
                    }
                }
            });

            image.Save(memoryStream, new PngEncoder());
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        private static Stream SaveAdditionalData(ArtObject ao)
        {
            var json = CustomJsonSerializer.Serialize(ArtObjectJsonData.FromArtObject(ao));
            return new MemoryStream(Encoding.UTF8.GetBytes(json));
        }

        private static void LoadGeometry(Stream geometryStream, ref ArtObject ao)
        {
            using var geometryReader = new BinaryReader(geometryStream, Encoding.UTF8, true);
            string geometryString = new string(geometryReader.ReadChars((int)geometryStream.Length));
            var geometry = WavefrontObjUtil.FromWavefrontObj<Matrix>(geometryString);
            if(geometry.Count() > 0) ao.Geometry = geometry.Values.First();

            // recalculate geometry texture coordinates into correct cubemap coords
            foreach (var vertex in ao.Geometry.Vertices)
            {
                (int textureOffset, Vector3 xAxis, Vector3 yAxis) = vertex.NormalByte switch
                {
                    5 => (0, new Vector3(1, 0, 0), new Vector3(0, -1, 0)), // front
                    3 => (1, new Vector3(0, 0, -1), new Vector3(0, -1, 0)), // right
                    2 => (2, new Vector3(-1, 0, 0), new Vector3(0, -1, 0)), // back
                    0 => (3, new Vector3(0, 0, 1), new Vector3(0, -1, 0)), // left
                    4 => (4, new Vector3(1, 0, 0), new Vector3(0, 0, 1)), // top
                    1 => (5, new Vector3(1, 0, 0), new Vector3(0, 0, -1)), // bottom
                };

                var texturePlanePosition = vertex.Position / ao.Size;
                vertex.TextureCoordinate = new Vector2(
                    (Vector3.Dot(texturePlanePosition, xAxis) + 0.5f + textureOffset) / 6.0f,
                    Vector3.Dot(texturePlanePosition, yAxis) + 0.5f
                );
            }
        }

        private static void LoadCubemap(Stream imageStream, ref ArtObject ao)
        {
            using var importedImage = Image.Load<Rgba32>(imageStream);
            ao.Cubemap = TextureConverter.ImageToTexture2D(importedImage);
        }

        private static void LoadAdditionalData(Stream jsonStream, ref ArtObject ao)
        {
            using var jsonReader = new BinaryReader(jsonStream, Encoding.UTF8, true);
            string json = new string(jsonReader.ReadChars((int)jsonStream.Length));
            var data = CustomJsonSerializer.Deserialize<ArtObjectJsonData>(json);

            ao.Name = data.Name;
            ao.Size = data.Size;
            ao.ActorType = data.ActorType;
            ao.NoSihouette = data.NoSihouette;
        }
    }
}
