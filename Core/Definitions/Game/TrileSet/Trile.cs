using System.Text.Json.Serialization;

using FEZRepacker.Core.Definitions.Game.ArtObject;
using FEZRepacker.Core.Definitions.Game.Common;
using FEZRepacker.Core.Definitions.Game.Graphics;
using FEZRepacker.Core.Definitions.Game.XNA;

namespace FEZRepacker.Core.Definitions.Game.TrileSet
{
    [XnbType("FezEngine.Structure.Trile")]
    [XnbReaderType("FezEngine.Readers.TrileReader")]
    public class Trile
    {
        [XnbProperty]
        public string Name { get; set; } = "";

        [XnbProperty]
        public string CubemapPath { get; set; } = "";

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
        public Dictionary<FaceOrientation, CollisionType> Faces { get; set; } = new();

        [JsonIgnore]
        [XnbProperty(UseConverter = true)]
        public IndexedPrimitives<VertexInstance, Vector4> Geometry { get; set; } = new();

        [XnbProperty(UseConverter = true)]
        public ActorType Type { get; set; }

        [XnbProperty(UseConverter = true)]
        public FaceOrientation Face { get; set; }

        [XnbProperty(UseConverter = true)]
        public SurfaceType SurfaceType { get; set; }

        [XnbProperty]
        public Vector2 AtlasOffset { get; set; }

    }
}
