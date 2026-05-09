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
}
