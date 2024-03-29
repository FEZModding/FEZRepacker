﻿namespace FEZRepacker.Core.Definitions.Game.Level
{
    [XnbType("FezEngine.Structure.CameraNodeData")]
    [XnbReaderType("FezEngine.Readers.CameraNodeDataReader")]
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
