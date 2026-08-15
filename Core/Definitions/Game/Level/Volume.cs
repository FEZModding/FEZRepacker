using FEZRepacker.Core.Definitions.Game.Common;
using FEZRepacker.Core.Definitions.Game.XNA;

namespace FEZRepacker.Core.Definitions.Game.Level
{
    [XnbType("FezEngine.Structure.Volume, FezEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [XnbReaderType("FezEngine.Readers.VolumeReader, FezEngine")]
    public class Volume
    {
        [XnbProperty(UseConverter = true)]
        public FaceOrientation[] Orientations { get; set; } = { };

        [XnbProperty]
        public Vector3 From { get; set; }

        [XnbProperty]
        public Vector3 To { get; set; }

        [XnbProperty(UseConverter = true)]
        public VolumeActorSettings? ActorSettings { get; set; }
    }
}
