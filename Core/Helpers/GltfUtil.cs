using System.Text.Json.Nodes;

using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;

using FEZRepacker.Core.Definitions.Game.ArtObject;
using FEZRepacker.Core.Definitions.Game.Graphics;
using FEZRepacker.Core.Helpers.Json;

using SharpGLTF.Schema2;
using SharpGLTF.Transforms;
using SharpGLTF.Validation;

using SixLabors.ImageSharp.Formats.Png;

namespace FEZRepacker.Core.Helpers
{
    using RgbaImage = SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32>;
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
        
        private const string PrimitiveTypeKey = "PrimitiveType";

        private static readonly Dictionary<GltfPrimitiveType, XnaPrimitiveType> PrimitiveTypeLookup = new()
        {
            { GltfPrimitiveType.LINES, XnaPrimitiveType.LineList },
            { GltfPrimitiveType.LINE_STRIP, XnaPrimitiveType.LineStrip },
            { GltfPrimitiveType.TRIANGLES, XnaPrimitiveType.TriangleList },
            { GltfPrimitiveType.TRIANGLE_STRIP, XnaPrimitiveType.TriangleStrip }
        };

        private static readonly Dictionary<XnaPrimitiveType, GltfPrimitiveType> PrimitiveTypeReverseLookup = new()
        {
            { XnaPrimitiveType.LineList, GltfPrimitiveType.LINES },
            { XnaPrimitiveType.LineStrip, GltfPrimitiveType.LINE_STRIP },
            { XnaPrimitiveType.TriangleList, GltfPrimitiveType.TRIANGLES },
            { XnaPrimitiveType.TriangleStrip, GltfPrimitiveType.TRIANGLE_STRIP }
        };

        public static ModelRoot ToGltfModel<T>(
            string name,
            List<GltfEntry<T>> entries,
            RgbaImage albedo,
            RgbaImage emission)
        {
            var steps = 0;
            var translation = XnaVector3.Zero;
            
            var model = ModelRoot.CreateModel();
            var scene = model.UseScene(name);
            var material = CreateMaterial(model, albedo, emission);
            
            foreach (var entry in entries)
            {
                var node = scene.CreateNode(entry.Name);
                node.LocalTransform = new AffineTransform(System.Numerics.Quaternion.Identity, translation.ToNumeric());
                node.Mesh = CreateMesh(model, entry.Geometry, material);
                node.Extras = entry.Extras ?? new JsonObject();
                node.Extras[PrimitiveTypeKey] = ConfiguredJsonSerializer.SerializeToNode(entry.Geometry.PrimitiveType);

                steps++;
                translation.X += StepOffsetX;
                if (steps % NewLineStep != 0) continue;
                translation.X = 0;
                translation.Z += StepOffsetZ;
            }

            return model;
        }

        public static ModelRoot ToGltfModel<T>(
            GltfEntry<T> entry,
            RgbaImage albedo,
            RgbaImage emission)
        {
            var model = ModelRoot.CreateModel();
            var scene = model.UseScene(entry.Name);
            var material = CreateMaterial(model, albedo, emission);
            
            var node = scene.CreateNode(entry.Name);
            node.LocalTransform = AffineTransform.Identity;
            node.Mesh = CreateMesh(model, entry.Geometry, material);
            node.Extras = entry.Extras ?? new JsonObject();
            node.Extras[PrimitiveTypeKey] = ConfiguredJsonSerializer.SerializeToNode(entry.Geometry.PrimitiveType);

            return model;
        }
        
        public static List<GltfEntry<T>> FromGltfModel<T>(ModelRoot model)
        {
            var geometryList = new List<GltfEntry<T>>();

            foreach (var node in model.LogicalNodes)
            {
                if (node.Mesh == null)
                {
                    var jsonNode = node.Extras[PrimitiveTypeKey];
                    var primitiveType = ConfiguredJsonSerializer.DeserializeFromNode<XnaPrimitiveType>(jsonNode);
                    var instance = new IndexedPrimitives<VertexInstance, T> { PrimitiveType = primitiveType };
                    geometryList.Add(new GltfEntry<T>(node.Name, instance, node.Extras));
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

        private static Material CreateMaterial(
            ModelRoot root,
            RgbaImage albedo,
            RgbaImage emission)
        {
            ImageBuilder albedoImage = albedo.SaveAsMemoryStream(new PngEncoder()).ToArray();
            ImageBuilder emissionImage = emission.SaveAsMemoryStream(new PngEncoder()).ToArray();

            var materialBuilder = new MaterialBuilder()
                .WithMetallicRoughnessShader();
            
            materialBuilder
                .UseChannel(KnownChannel.BaseColor)
                .UseTexture()
                .WithPrimaryImage(albedoImage)
                .WithSampler(
                    TextureWrapMode.REPEAT,
                    TextureWrapMode.REPEAT,
                    TextureMipMapFilter.NEAREST,
                    TextureInterpolationFilter.NEAREST);
            
            materialBuilder
                .UseChannel(KnownChannel.Emissive)
                .UseTexture()
                .WithPrimaryImage(emissionImage)
                .WithSampler(
                    TextureWrapMode.REPEAT,
                    TextureWrapMode.REPEAT,
                    TextureMipMapFilter.NEAREST,
                    TextureInterpolationFilter.NEAREST);

            return root.CreateMaterial(materialBuilder);
        }

        private static Mesh? CreateMesh<T>(
            ModelRoot root,
            IndexedPrimitives<VertexInstance, T> geometry,
            Material material)
        {
            if (geometry.Vertices.Length == 0)
            {
                // glTF's nodes may not store mesh data.
                // See: https://registry.khronos.org/glTF/specs/2.0/glTF-2.0.html#geometry-overview
                return null;
            }

            var primitiveType = PrimitiveTypeReverseLookup[geometry.PrimitiveType];

            var vertexAccessors = geometry.Vertices
                .Select(instance => instance.ToVertex())
                .ToArray();

            var indices = geometry.Indices
                .Select(i => (int)i)
                .ToArray();

            var mesh = root.CreateMesh();
            mesh.CreatePrimitive()
                .WithMaterial(material)
                .WithVertexAccessors(vertexAccessors)
                .WithIndicesAccessor(primitiveType, indices);

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

        private static (VertexPositionNormal, VertexTexture1) ToVertex(this VertexInstance instance)
        {
            return (
                new VertexPositionNormal(instance.Position.ToNumeric(), instance.Normal.ToNumeric()),
                new VertexTexture1(instance.TextureCoordinate.ToNumeric())
            );
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
        JsonNode? Extras);
}