using FEZRepacker.Core.Definitions.Game.Common;

namespace FEZRepacker.Core.Definitions.Game.Level
{
    [XnbType("FezEngine.Structure.TrileFace, FezEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [XnbReaderType("FezEngine.Readers.TrileFaceReader, FezEngine")]
    public class TrileFace
    {
        [XnbProperty(UseConverter = true)]
        public TrileEmplacement Id { get; set; } = new();

        [XnbProperty(UseConverter = true)]
        public FaceOrientation Face { get; set; }
    }
}
