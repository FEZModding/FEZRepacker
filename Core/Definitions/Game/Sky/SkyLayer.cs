namespace FEZRepacker.Core.Definitions.Game.Sky
{
    [XnbType("FezEngine.Structure.SkyLayer, FezEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [XnbReaderType("FezEngine.Readers.SkyLayerReader, FezEngine")]
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
