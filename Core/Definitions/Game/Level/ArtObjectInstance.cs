using FEZRepacker.Core.Definitions.Game.XNA;

namespace FEZRepacker.Core.Definitions.Game.Level
{
    [XnbType("FezEngine.Structure.ArtObjectInstance, FezEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [XnbReaderType("FezEngine.Readers.ArtObjectInstanceReader, FezEngine")]
    public class ArtObjectInstance
    {
        [XnbProperty]
        public string Name { get; set; } = "";

        [XnbProperty]
        public Vector3 Position { get; set; }

        [XnbProperty]
        public Quaternion Rotation { get; set; }

        [XnbProperty]
        public Vector3 Scale { get; set; }

        [XnbProperty(UseConverter = true)]
        public ArtObjectActorSettings? ActorSettings { get; set; }
    }
}
