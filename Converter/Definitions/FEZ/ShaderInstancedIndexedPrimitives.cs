using FEZRepacker.Converter.Definitions.MicrosoftXna;

namespace FEZRepacker.Converter.Definitions.FezEngine.Structure.Geometry
{
    [XnbType("FezEngine.Structure.Geometry.ShaderInstancedIndexedPrimitives")]
    [XnbReaderType("FezEngine.Readers.ShaderInstancedIndexedPrimitivesReader")]
    internal class ShaderInstancedIndexedPrimitives<TemplateType, InstanceType>
    {
        [XnbProperty(UseConverter = true)]
        public PrimitiveType PrimitiveType { get; set; }

        [XnbProperty(UseConverter = true)]
        public TemplateType[] Vertices { get; set; }

        [XnbProperty(UseConverter = true)]
        public ushort[] Indices { get; set; }

        public ShaderInstancedIndexedPrimitives(){
            Vertices = new TemplateType[0];
            Indices = new ushort[0];
        }
    }
}
