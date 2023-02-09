using System.Numerics;

using FEZRepacker.Converter.Definitions.FezEngine.Structure.Geometry;
using FEZRepacker.Converter.Definitions.MicrosoftXna;

namespace FEZRepacker.Converter.Definitions.FezEngine.Structure
{
    [XnbType("FezEngine.Structure.ArtObject")]
    [XnbReaderType("FezEngine.Readers.ArtObjectReader")]
    internal class ArtObject
    {
        [XnbProperty]
        public string Name { get; set; }

        [XnbProperty(UseConverter = true)]
        public Texture2D Cubemap { get; set; }

        [XnbProperty]
        public Vector3 Size { get; set; }

        [XnbProperty(UseConverter = true)]
        public ShaderInstancedIndexedPrimitives<VertexPositionNormalTextureInstance, Matrix> Geometry { get; set; }

        [XnbProperty(UseConverter = true)]
        public ActorType ActorType { get; set; }

        [XnbProperty]
        public bool NoSihouette { get; set; }
    }
}
