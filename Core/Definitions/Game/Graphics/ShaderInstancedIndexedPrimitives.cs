using FEZRepacker.Core.Definitions.Game.XNA;

namespace FEZRepacker.Core.Definitions.Game.Graphics
{
    [XnbType("FezEngine.Structure.Geometry.ShaderInstancedIndexedPrimitives")]
    [XnbReaderType("FezEngine.Readers.ShaderInstancedIndexedPrimitivesReader")]
    public class ShaderInstancedIndexedPrimitives<TemplateType, InstanceType>
    {
        [XnbProperty(UseConverter = true)]
        public PrimitiveType PrimitiveType { get; set; }

        [XnbProperty(UseConverter = true)]
        public TemplateType[] Vertices { get; set; }

        [XnbProperty(UseConverter = true)]
        public ushort[] Indices { get; set; }

        public ShaderInstancedIndexedPrimitives()
        {
            Vertices = new TemplateType[0];
            Indices = new ushort[0];
        }
    }
}
