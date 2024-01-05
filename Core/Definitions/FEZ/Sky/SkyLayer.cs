namespace FEZRepacker.Converter.Definitions.FezEngine.Structure
{
    [XnbType("FezEngine.Structure.SkyLayer")]
    [XnbReaderType("FezEngine.Readers.SkyLayerReader")]
    internal class SkyLayer
    {
        [XnbProperty]
        public string Name { get; set; }

        [XnbProperty]
        public bool InFront { get; set; }

        [XnbProperty]
        public float Opacity { get; set; }

        [XnbProperty]
        public float FogTint { get; set; }
    }
}
