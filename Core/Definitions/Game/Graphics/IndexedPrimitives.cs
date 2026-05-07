using FEZRepacker.Core.Definitions.Game.ArtObject;
using FEZRepacker.Core.Definitions.Game.XNA;

namespace FEZRepacker.Core.Definitions.Game.Graphics
{
    [XnbType("FezEngine.Structure.Geometry.ShaderInstancedIndexedPrimitives, FezEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [XnbReaderType("FezEngine.Readers.ShaderInstancedIndexedPrimitivesReader, FezEngine")]
    // Original name in FezEngine: ShaderInstancedIndexedPrimitives
    public class IndexedPrimitives<TemplateType, InstanceType> 
    {
        [XnbProperty(UseConverter = true)]
        public PrimitiveType PrimitiveType { get; set; }

        [XnbProperty(UseConverter = true)]
        public TemplateType[] Vertices { get; set; }

        [XnbProperty(UseConverter = true)]
        public ushort[] Indices { get; set; }

        public IndexedPrimitives()
        {
            Vertices = new TemplateType[0];
            Indices = new ushort[0];
        }
    }
    
    
    /* Helper subclasses for serialization sourcegen - easier to control qualifier name this way. */

    [XnbReaderType("FezEngine.Readers.ShaderInstancedIndexedPrimitivesReader`2" +
                   "[[FezEngine.Structure.Geometry.VertexPositionNormalTextureInstance, FezEngine]," +
                   "[Microsoft.Xna.Framework.Matrix, Microsoft.Xna.Framework, Version=4.0.0.0, " +
                   "Culture=neutral, PublicKeyToken=842cf8be1de50553]], FezEngine", 
        UseBaseClass = true)]
    internal class ArtObjectIndexedPrimitives : IndexedPrimitives<VertexInstance, Matrix>;
    
    [XnbReaderType("FezEngine.Readers.ShaderInstancedIndexedPrimitivesReader`2" +
                   "[[FezEngine.Structure.Geometry.VertexPositionNormalTextureInstance, FezEngine]," +
                   "[Microsoft.Xna.Framework.Vector4, Microsoft.Xna.Framework, Version=4.0.0.0, " +
                   "Culture=neutral, PublicKeyToken=842cf8be1de50553]], FezEngine", 
        UseBaseClass = true)]
    internal class TrileSetIndexedPrimitives : IndexedPrimitives<VertexInstance, Vector4>;
}
