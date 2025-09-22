using System.Text.Json.Nodes;

using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;

using FEZRepacker.Core.Definitions.Game.ArtObject;
using FEZRepacker.Core.Definitions.Game.Graphics;
using FEZRepacker.Core.XNB;

using SharpGLTF.Scenes;
using SharpGLTF.Schema2;
using SharpGLTF.Validation;

using SixLabors.ImageSharp.Formats.Png;

namespace FEZRepacker.Core.Helpers
{
    using RgbaImage = SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32>;
    using GltfVertex = VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>;
    using GltfMesh = MeshBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>;
    using GltfPrimitiveType = PrimitiveType;
    using XnaPrimitiveType = Definitions.Game.XNA.PrimitiveType;
    using XnaVector3 = Definitions.Game.XNA.Vector3;
    using XnaVector2 = Definitions.Game.XNA.Vector2;

    internal static class GltfUtil
    {
        private const float NewLineStep = 15;

        private const float StepOffsetX = 2f;

        private const float StepOffsetZ = -2f;

        private const int TempFileBufferSize = 4096;

        private static readonly Dictionary<GltfPrimitiveType, XnaPrimitiveType> PrimitiveTypeLookup = new()
        {
            { GltfPrimitiveType.LINES, XnaPrimitiveType.LineList },
            { GltfPrimitiveType.LINE_STRIP, XnaPrimitiveType.LineStrip },
            { GltfPrimitiveType.TRIANGLES, XnaPrimitiveType.TriangleList },
            { GltfPrimitiveType.TRIANGLE_STRIP, XnaPrimitiveType.TriangleList }
        };

        public static ModelRoot ToGltfModel<T>(
            string name,
            List<GltfEntry<T>> entries,
            RgbaImage albedo,
            RgbaImage emission)
        {
            var steps = 0;
            var translation = XnaVector3.Zero;
            
            var scene = new SceneBuilder(name);
            var material = CreateMaterial(albedo, emission);
            
            foreach (var dto in entries)
            {
                var node = new NodeBuilder(dto.Name).WithLocalTranslation(translation.ToNumeric());
                node.Extras = dto.Extras;
                
                var mesh = CreateMesh(dto.Geometry, material);
                scene.AddRigidMesh(mesh, node);

                steps++;
                translation.X += StepOffsetX;
                if (steps % NewLineStep != 0) continue;
                translation.X = 0;
                translation.Z += StepOffsetZ;
            }

            return scene.ToGltf2();
        }

        public static ModelRoot ToGltfModel<T>(
            GltfEntry<T> gltfEntry,
            RgbaImage albedo,
            RgbaImage emission)
        {
            var scene = new SceneBuilder(gltfEntry.Name);
            var node = new NodeBuilder(gltfEntry.Name);
            
            var material = CreateMaterial(albedo, emission);
            var mesh = CreateMesh(gltfEntry.Geometry, material);
            
            node.Extras = gltfEntry.Extras;
            scene.AddRigidMesh(mesh, node);
            return scene.ToGltf2();
        }
        
        public static List<GltfEntry<T>> FromGltfModel<T>(ModelRoot model)
        {
            var geometryList = new List<GltfEntry<T>>();

            foreach (var node in model.LogicalNodes)
            {
                if (node.Mesh == null)
                {
                    geometryList.Add(new GltfEntry<T>(node.Name, new IndexedPrimitives<VertexInstance, T>(), node.Extras));
                    continue;
                }
                
                var primitive = node.Mesh.Primitives[0];
                var positions = primitive.GetVertexAccessor("POSITION").AsVector3Array();
                var normals = primitive.GetVertexAccessor("NORMAL").AsVector3Array();
                var textureCoords = primitive.GetVertexAccessor("TEXCOORD_0").AsVector2Array();
                var indices = primitive.GetIndexAccessor().AsIndicesArray();

                var indexedPrimitives = new IndexedPrimitives<VertexInstance, T>
                {
                    PrimitiveType = PrimitiveTypeLookup[primitive.DrawPrimitiveType],
                    Vertices = new VertexInstance[positions.Count],
                    Indices = new ushort[indices.Count]
                };

                for (var i = 0; i < positions.Count; i++)
                {
                    var instance = ToVertexInstance(positions[i], normals[i], textureCoords[i]);
                    indexedPrimitives.Vertices[i] = instance;
                }

                for (var i = 0; i < indices.Count; i++)
                {
                    indexedPrimitives.Indices[i] = (ushort)indices[i];
                }
                
                geometryList.Add(new GltfEntry<T>(node.Name, indexedPrimitives, node.Extras));
            }

            return geometryList;
        }

        public static (Stream?, Stream?) ExtractCubemapStreams(ModelRoot model)
        {
            if (model.LogicalMaterials.Count < 1)
            {
                return default;
            }
            
            var material = model.LogicalMaterials[0];
            var albedoChannel = material.FindChannel("BaseColor");
            var emissionChannel = material.FindChannel("Emissive");
            
            var albedoMemoryImage = albedoChannel?.Texture.PrimaryImage.Content;
            var emissionMemoryImage = emissionChannel?.Texture.PrimaryImage.Content;

            var albedoStream = albedoMemoryImage?.Open();
            var emissionStream = emissionMemoryImage?.Open();
            
            return (albedoStream, emissionStream);
        }

        private static MaterialBuilder CreateMaterial(
            RgbaImage albedo,
            RgbaImage emission)
        {
            ImageBuilder albedoImage = albedo.SaveAsMemoryStream(new PngEncoder()).ToArray();
            ImageBuilder emissionImage = emission.SaveAsMemoryStream(new PngEncoder()).ToArray();

            var material = new MaterialBuilder()
                .WithDoubleSide(true)
                .WithMetallicRoughnessShader();
            
            material
                .UseChannel(KnownChannel.BaseColor)
                .UseTexture()
                .WithPrimaryImage(albedoImage)
                .WithSampler(
                    TextureWrapMode.REPEAT,
                    TextureWrapMode.REPEAT,
                    TextureMipMapFilter.NEAREST,
                    TextureInterpolationFilter.NEAREST);
            
            material
                .UseChannel(KnownChannel.Emissive)
                .UseTexture()
                .WithPrimaryImage(emissionImage)
                .WithSampler(
                    TextureWrapMode.REPEAT,
                    TextureWrapMode.REPEAT,
                    TextureMipMapFilter.NEAREST,
                    TextureInterpolationFilter.NEAREST);

            return material;
        }

        private static GltfMesh CreateMesh<T>(
            IndexedPrimitives<VertexInstance, T> geometry,
            MaterialBuilder material)
        {
            var mesh = GltfVertex.CreateCompatibleMesh();
            var primitive = mesh.UsePrimitive(material);

            var indices = geometry.Indices;
            var isLine = geometry.PrimitiveType is XnaPrimitiveType.LineList or XnaPrimitiveType.LineStrip;
            var isList = geometry.PrimitiveType is XnaPrimitiveType.TriangleList or XnaPrimitiveType.LineList;

            if (isLine)
            {
                for (int i = 0; i < indices.Length - indices.Length % 2; i += (isList ? 2 : 1))
                {
                    var v1 = geometry.Vertices[indices[i]];
                    var v2 = geometry.Vertices[indices[i + 1]];
                    primitive.AddLine(v1.ToVertex(), v2.ToVertex());
                }
            }
            else
            {
                for (int i = 0; i < indices.Length - indices.Length % 3; i += (isList ? 3 : 1))
                {
                    var v1 = geometry.Vertices[indices[i]];
                    var v2 = geometry.Vertices[indices[i + 1]];
                    var v3 = geometry.Vertices[indices[i + 2]];
                    primitive.AddTriangle(v1.ToVertex(), v2.ToVertex(), v3.ToVertex());
                }
            }

            return mesh;
        }

        public static MemoryStream SaveAsGlb(this ModelRoot model)
        {
            var tempPath = Path.GetTempFileName();
            model.Asset.Generator = Metadata.Version;
            model.SaveGLB(tempPath, new WriteSettings
            {
                Validation = ValidationMode.Strict,
                MergeBuffers = true
            });

            var memoryStream = new MemoryStream();
            using var fileStream = new FileStream(tempPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.None,
                TempFileBufferSize,
                FileOptions.DeleteOnClose);
            fileStream.CopyTo(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        private static GltfVertex ToVertex(this VertexInstance instance)
        {
            return GltfVertex
                .Create(instance.Position.ToNumeric(), instance.Normal.ToNumeric())
                .WithMaterial(instance.TextureCoordinate.ToNumeric());
        }

        private static VertexInstance ToVertexInstance(
            System.Numerics.Vector3 position,
            System.Numerics.Vector3 normal,
            System.Numerics.Vector2 textureCoordinate)
        {
            return new VertexInstance
            {
                Position = XnaVector3.FromNumeric(position),
                Normal = XnaVector3.FromNumeric(normal),
                TextureCoordinate = XnaVector2.FromNumeric(textureCoordinate)
            };
        }
    }
    
    /// <summary>
    /// Data Transfer Object (DTO) for <see cref="GltfUtil"/>.
    /// </summary>
    public record struct GltfEntry<TInstanceType>(
        string Name,
        IndexedPrimitives<VertexInstance, TInstanceType> Geometry,
        JsonNode Extras);
}