namespace FEZRepacker.Core.Definitions.Game.Sky
{
    [XnbType("FezEngine.Structure.SkyLayer")]
    [XnbReaderType("FezEngine.Readers.SkyLayerReader")]
    public class SkyLayer
    {
        [XnbProperty]
        public string Name { get; set; } = "";

        [XnbProperty]
        public bool InFront { get; set; }

        [XnbProperty]
        public float Opacity { get; set; }

        [XnbProperty]
        public float FogTint { get; set; }
    }
}
