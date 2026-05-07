namespace FEZRepacker.Core.Definitions.Game.Level
{
    [XnbType("FezEngine.Structure.CameraNodeData, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [XnbReaderType("FezEngine.Readers.CameraNodeDataReader, FezEngine")]
    public class CameraNodeData
    {
        [XnbProperty]
        public bool Perspective { get; set; }

        [XnbProperty]
        public int PixelsPerTrixel { get; set; }

        [XnbProperty(UseConverter = true)]
        public string SoundName { get; set; } = "";
    }
}
