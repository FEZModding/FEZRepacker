﻿using System.Numerics;
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
            var albedoTexture = SaveCubemap(ao, false);
            var emissionTexture = SaveCubemap(ao, true);
            var data = SaveAdditionalData(ao);

            var bundle = new FileBundle(FileFormat);
            bundle.AddFile(geometry, ".obj");
            bundle.AddFile(albedoTexture, ".png");
            bundle.AddFile(emissionTexture, ".apng");
            bundle.AddFile(data, ".json");
            return bundle;
        }

        public override void WriteXnbContent(FileBundle bundle, BinaryWriter xnbWriter)
        {
            ArtObject ao = new ArtObject();

            Stream albedoData = null;
            Stream emissionData = null;

            foreach(var file in bundle.Files)
            {
                if (file.Extension == ".obj") LoadGeometry(file.Data, ref ao);
                if (file.Extension == ".png") albedoData = file.Data;
                if (file.Extension == ".apng") emissionData = file.Data;
                if (file.Extension == ".json") LoadAdditionalData(file.Data, ref ao);
            }

            LoadCubemap(albedoData, emissionData, ref ao);

            PrimaryContentType.Write(ao, xnbWriter);
        }



        private static Stream SaveGeometry(ArtObject ao)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(ao.Geometry.ToWavefrontObj()));
        }

        private static Stream SaveCubemap(ArtObject ao, bool emission)
        {
            var memoryStream = new MemoryStream();

            if (ao.Cubemap.TextureData.Length == 0)
            {
                return memoryStream;
            }
            
            using var image = TextureConverter.ImageFromTexture2D(ao.Cubemap);

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

        private static void LoadCubemap(Stream imageAlbedoStream, Stream imageEmissionStream, ref ArtObject ao)
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

            ao.Cubemap = TextureConverter.ImageToTexture2D(albedoImage);
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
