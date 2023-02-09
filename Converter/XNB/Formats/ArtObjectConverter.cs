using System.Numerics;
using System.Text;

using FEZRepacker.Converter.Definitions.FezEngine.Structure;
using FEZRepacker.Converter.Definitions.FezEngine.Structure.Geometry;
using FEZRepacker.Converter.Definitions.MicrosoftXna;
using FEZRepacker.Converter.XNB.Formats.Json;
using FEZRepacker.Converter.XNB.Formats.Json.CustomStructures;
using FEZRepacker.Converter.XNB.Types;
using FEZRepacker.Converter.XNB.Types.System;

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
        public override string FileFormat => "#fezao";

        public override void FromBinary(BinaryReader xnbReader, BinaryWriter outWriter)
        {
            ArtObject ao = (ArtObject)PrimaryContentType.Read(xnbReader);

            var geometry = SaveGeometry(ao);
            var texture = SaveCubemap(ao);
            var data = SaveAdditionalData(ao);

            // TODO: implement conversion
        }

        public override void ToBinary(BinaryReader inReader, BinaryWriter xnbWriter)
        {
            ArtObject ao = new ArtObject();

            // TODO: implement conversion

            PrimaryContentType.Write(ao, xnbWriter);
        }



        private static byte[] SaveGeometry(ArtObject ao)
        {
            if (ao.Geometry.Vertices.Length == 0 || ao.Geometry.Indices.Length == 0)
            {
                return new byte[0];
            }

            var memoryStream = new MemoryStream();
            using var writer = new BinaryWriter(memoryStream);

            var vertices = new List<Vector3>();
            var textureCoordinates = new List<Vector2>();
            var normals = new List<Vector3>();

            foreach (var vertexData in ao.Geometry.Vertices)
            {
                vertices.Add(vertexData.Position);
                textureCoordinates.Add(vertexData.TextureCoordinate);
                normals.Add(vertexData.Normal);
            }

            foreach (var vertex in vertices)
            {
                writer.Write($"v {vertex.X} {vertex.Y} {vertex.Z}\n".ToCharArray());
            }

            foreach (var texCoord in textureCoordinates)
            {
                // V coords are upside down.
                writer.Write($"vt {texCoord.X} {-texCoord.Y}\n".ToCharArray());
            }

            foreach (var normal in normals)
            {
                // TODO: something's wrong with normals. fix them.
                writer.Write($"vn {normal.X} {normal.Y} {normal.Z}\n".ToCharArray());
            }

            var indices = ao.Geometry.Indices;
            var type = ao.Geometry.PrimitiveType;

            bool isLine = (type == PrimitiveType.LineList || type == PrimitiveType.LineStrip);
            bool isList = (type == PrimitiveType.TriangleList || type == PrimitiveType.LineList);

            if (isLine)
            {
                for (int i = 1; i < indices.Count() - indices.Count() % 2; i += (isList ? 2 : 1))
                {
                    writer.Write($"l {indices[i - 1] + 1} {indices[i] + 1}\n".ToCharArray());
                }
            }
            else
            {
                for (int i = 2; i < indices.Count() - indices.Count() % 3; i += (isList ? 3 : 1))
                {
                    int i1 = indices[i - 2] + 1;
                    int i2 = indices[i - 1] + 1;
                    int i3 = indices[i] + 1;

                    writer.Write($"f {i1}/{i1}/{i1} {i2}/{i2}/{i2} {i3}/{i3}/{i3}\n".ToCharArray());
                }
            }

            return memoryStream.ToArray();
        }

        private static byte[] SaveCubemap(ArtObject ao)
        {
            if (ao.Cubemap.TextureData.Length == 0)
            {
                return new byte[0];
            }

            var memoryStream = new MemoryStream();
            using var image = TextureConverter.ImageFromTexture2D(ao.Cubemap);

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
            return memoryStream.ToArray();
        }

        private static byte[] SaveAdditionalData(ArtObject ao)
        {
            var json = CustomJsonSerializer.Serialize(ArtObjectJsonData.FromArtObject(ao));
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
