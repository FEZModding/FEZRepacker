
using System.Numerics;
using System.Text.Json.Serialization;

using FEZRepacker.Converter.Definitions.FezEngine.Structure.Geometry;
using FEZRepacker.Converter.Definitions.MicrosoftXna;

using Vector4 = FEZRepacker.Converter.Definitions.MicrosoftXna.Vector4;

namespace FEZRepacker.Converter.Definitions.FezEngine.Structure
{
    [XnbType("FezEngine.Structure.Trile")]
    [XnbReaderType("FezEngine.Readers.TrileReader")]
    internal class Trile
    {
        [XnbProperty]
        public string Name { get; set; }

        [XnbProperty]
        public string CubemapPath { get; set; }

        [XnbProperty]
        public Vector3 Size { get; set; }

        [XnbProperty]
        public Vector3 Offset { get; set; }

        [XnbProperty]
        public bool Immaterial { get; set; }

        [XnbProperty]
        public bool SeeThrough { get; set; }

        [XnbProperty]
        public bool Thin { get; set; }

        [XnbProperty]
        public bool ForceHugging { get; set; }

        [XnbProperty(UseConverter = true)]
        public Dictionary<FaceOrientation, CollisionType> Faces { get; set; }

        [JsonIgnore]
        [XnbProperty(UseConverter = true)]
        public ShaderInstancedIndexedPrimitives<VertexPositionNormalTextureInstance, Vector4> Geometry { get; set; }

        [XnbProperty(UseConverter = true)]
        public ActorType Type { get; set; }

        [XnbProperty(UseConverter = true)]
        public FaceOrientation Face { get; set; }

        [XnbProperty(UseConverter = true)]
        public SurfaceType SurfaceType { get; set; }

        [XnbProperty]
        public Vector2 AtlasOffset { get; set; }

        
        public Trile()
        {
            Faces = new Dictionary<FaceOrientation, CollisionType>();
            Geometry = new ShaderInstancedIndexedPrimitives<VertexPositionNormalTextureInstance, Vector4>();
        }
    }
}
